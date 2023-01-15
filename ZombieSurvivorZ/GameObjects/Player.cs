using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{
    public class Player : SpriteColliderObject
    {

        private const float BaseColliderSize = 25;

        private const float MovementSpeed = 300;
        private const bool SameKeyHolstersWeapon = true;
        private const int MaterialGainPerZombie = 25;

        private const int MaxHealth = 100;
        private const float KnockbackOnHit = 12;

        private Reticle reticle;
        private Weapon weapon;

        private Texture2D bodyTexture;

        private readonly List<Weapon> weapons = new();
        private readonly Dictionary<int, Weapon> keyNumToWeapon = new();
        private int keyNumCount = 1;
        public int Materials { get; private set; }

        private int health;

        private bool hasDebugGun = false;


        private Vector2Int lastCellPos;
        public Vector2Int CellPosition => lastCellPos;

        public bool DoorSelected = false;
        public Vector2Int SelectingDoor;
        //Door Prompts & Crafting prompts
        private Texture2D UsePromptTexture;

        public bool CraftingSelected = false;
        public Vector2Int SelectingCrafting;

        private Texture2D BarricadePromptTexture;

        public float ColliderSize => Scale.X * BaseColliderSize;

        public const float BarricadeTime = 0.75f;
        public const int BarricadeMaterialCost = 5;
        private float barricadeTimeCount;


        public bool hasDied = false;
        public float timeSurvived = 0;


        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                CL.UpdatePosition(value);
            }
        }

        public Player()
        {
            CL = new CircleDynamicCollider(this, BaseColliderSize);
            health = MaxHealth;
        }

        public override void Initialize()
        {
            Texture = Game1.GetTexture("Player/player_head");
            bodyTexture = Game1.GetTexture("Player/player_body");

            UsePromptTexture = Game1.GetTexture("Keys/toggle_door");
            BarricadePromptTexture = Game1.GetTexture("Keys/barricade");

            RotationOffset = 90 * MathF.PI / 180;
            Position = new(0, 0);

            //Scale = new(0.5f, 0.5f);

            Pistol pistol = CreateWeapon(new Pistol());
            OwnWeapon(pistol);

            Rifle rifle = CreateWeapon(new Rifle());
            //OwnWeapon(rifle);
            Shotgun shotgun = CreateWeapon(new Shotgun());

            AddWeaponsToWeaponUpgradeUI();

            reticle = new();
            reticle.Disable();

            AddMaterials(100);  //Initial Materials

            Game1.HUDDisplayUI.PlayerHealthDisplayUI.HealthUpdated(health, MaxHealth);
        }

        protected override void ScaleChanged()
        {
            base.ScaleChanged();
            ((CircleDynamicCollider)CL).Set(ColliderSize);
            if (weapon != null)
            {
                weapon.Scale = Scale;
            }
        }

        public override void Update()
        {
            if (hasDied)
            {
                return;
            }
            timeSurvived += Time.deltaTime;

            TransformUpdate();
            WeaponOperationUpdate();
            WeaponUpdate();
            InteractionUpdate();
            CheatUpdate();
        }

        private void TransformUpdate()
        {
            Vector2 movement = Input.ConstructAxis2(Keys.S, Keys.W, Keys.D, Keys.A);
            if (movement.X == 0 && movement.Y == 0)
            {

            }
            else
            {
                movement.Normalize();
            }
            Position += movement * (MovementSpeed * Time.deltaTime);
            Heading = Game1.Camera.ScreenToWorld(reticle.Position) - Position;
        }

        private float gracePeriodHealTime = 1f;
        private float gracePeriodHealTimeCount;
        private int gracePeriodHeal = 1;

        public void IsGracePeriodUpdate()
        {
            gracePeriodHealTimeCount -= Time.deltaTime;
            if (gracePeriodHealTimeCount < 0)
            {
                health = Math.Min(MaxHealth, health + gracePeriodHeal);
                gracePeriodHealTimeCount = gracePeriodHealTime;
                Game1.HUDDisplayUI.PlayerHealthDisplayUI.HealthUpdated(health, MaxHealth);
            }
        }

        public void DealDamage(int damage, Vector2 direction)
        {
            Game1.BloodManager.AddHumanBlood(Position, MathF.Atan2(direction.Y, direction.X));
            Position += direction * KnockbackOnHit;

            health -= damage;
            if (health <= 0)
            {
                health = 0;
                Die();
            }
            Game1.HUDDisplayUI.PlayerHealthDisplayUI.HealthUpdated(health, MaxHealth);
        }

        private void Die()
        {
            Console.WriteLine("PLAYER DIED!!");
            hasDied = true;
            Active = false;
            Destroy();
            Game1.ScoreDisplayUI.PlayerDied(Game1.MapManager.ZombieSpawnLayer.zombiesKilled, new(0, 0, 0, 0, (int)(timeSurvived * 1000)));
        }

        #region Materials

        private void AddMaterials(int materials)
        {
            Materials += materials;
            Game1.HUDDisplayUI?.MaterialsDisplayUI.UpdateMaterials(Materials);
        }
        private void RemoveMaterials(int materials)
        {
            Materials -= materials;
            Game1.HUDDisplayUI.MaterialsDisplayUI.UpdateMaterials(Materials);
        }
        public void ZombieKilledAddMaterials()
        {
            AddMaterials(MaterialGainPerZombie);
        }
        public bool TryRemoveMaterials(int materials)
        {
            if (Materials >= materials)
            {
                RemoveMaterials(materials);
                return true;
            }
            return false;
        }

        #endregion

        #region Weapon Operation

        private T CreateWeapon<T>(T weapon) where T : Weapon
        {
            weapons.Add(weapon);
            weapon.IsOwned = false;
            return weapon;
        }
        public void OwnWeapon(Weapon weapon)
        {
            keyNumToWeapon.Add(keyNumCount++, weapon);
            weapon.IsOwned = true;
            weapon.UpdateWeaponUpgradeUI();
        }

        private void AddWeaponsToWeaponUpgradeUI()
        {
            foreach (Weapon weapon in weapons)
            {
                Game1.UpgradeWindowUI.Player_AddWeaponToList(weapon);
            }
        }

        private void WeaponOperationUpdate()
        {
            WeaponSwitchUpdate();
            WeaponReloadUpdate();
        }


        private void WeaponSwitchUpdate()
        {
            int numKey = Input.GetNumberKeysFirstDown(1, weapons.Count);
            if (numKey == -1)
            {
                return;
            }
            if (!keyNumToWeapon.TryGetValue(numKey, out Weapon weapon))
            {
                SwitchToWeapon(null);
                return;
            }
            SwitchToWeapon(weapon);
        }

        private void WeaponReloadUpdate()
        {
            if (weapon == null)
            {
                return;
            }
            if (Input.IsKeyFirstDown(Keys.R))
            {
                weapon.Reload();
            }
        }


        private void SwitchToWeapon(Weapon weapon)
        {
            Weapon previousWeapon = this.weapon;
            previousWeapon?.HolsterWeapon();
            if (SameKeyHolstersWeapon)
            {
                if (weapon == this.weapon)
                {
                    this.weapon = null;
                    reticle.Disable();
                    Game1.HUDDisplayUI.AmmoDisplayUI.WeaponNotActive();
                    return;
                }
            }
            this.weapon = weapon;

            if (weapon == null)
            {
                reticle.Disable();
                Game1.HUDDisplayUI.AmmoDisplayUI.WeaponNotActive();
                return;
            }
            weapon.Active = true;
            weapon.RotationOffset = 90 * MathF.PI / 180;
            weapon.HoldWeapon();

            reticle.Enable();
            Game1.HUDDisplayUI.AmmoDisplayUI.WeaponActive();
        }

        #endregion

        private void WeaponUpdate()
        {
            if (weapon == null)
            {
                return;
            }

            weapon.Position = Position;
            weapon.Heading = Heading;

            if (Input.IsLMouseFirstDown())
            {
                weapon.SemiFire();
                
            }
            else if (Input.IsLMouseDown())
            {
                weapon.AutoFire();
            }
            reticle.SetSpread(weapon.GetVisualRecoilSpread());
            Game1.HUDDisplayUI.AmmoDisplayUI.UpdateAmmoCount(weapon.AmmoInClip, weapon.AmmoReserve);
        }

        #region Interaction

        private void InteractionUpdate()
        {
            Vector2Int playerCell = Game1.MapManager.PositionToLocal(Position);
            if (playerCell != lastCellPos)
            {
                //Checks
                DoorCheck(playerCell);
                CraftingCheck(playerCell);

                lastCellPos = playerCell;
            }

            //Updates
            DoorUpdate();
            CraftingUpdate();
            BarricadeUpdate();
        }

        private void DoorUpdate()
        {
            if (!DoorSelected)
            {
                Game1.HUDDisplayUI.DoorHealthDisplayUI.SetActive(false);
                return;
            }
            if (Input.IsRMouseFirstDown())
            {
                Game1.MapManager.DoorsLayer.ToggleDoor(SelectingDoor);
            }
            Game1.HUDDisplayUI.DoorHealthDisplayUI.SetActive(true);
            Vector2 doorPos = Game1.MapManager.LocalToTileTopLeftPosition(SelectingDoor);
            Game1.HUDDisplayUI.DoorHealthDisplayUI.SetDoor(Game1.MapManager.DoorsLayer.Doors[SelectingDoor], doorPos);
        }

        private void BarricadeUpdate()
        {
            if (!DoorSelected)
            {
                barricadeTimeCount = 0;
                //if (SelectingDoor != new Vector2Int(0,0))
                //{
                //    // to stop barricading when the player stops selecting the door
                //    Game1.MapManager.DoorsLayer.Doors[SelectingDoor].ResetBarricading();
                //}
                return;
            }
            if (Input.IsKeyDown(Keys.Space))
            {

                if (Materials >= BarricadeMaterialCost)
                {
                    barricadeTimeCount += Time.deltaTime;
                    //Console.WriteLine(barricadeTimeCount);
                    if (Game1.MapManager.DoorsLayer.Doors[SelectingDoor].CanBarricade())
                    {
                        Game1.HUDDisplayUI.DoorHealthDisplayUI.SetBarricadeStatus(barricadeTimeCount, BarricadeTime);
                    }
                    if (barricadeTimeCount > BarricadeTime)
                    {
                        if (Game1.MapManager.DoorsLayer.TryBarricadeDoor(SelectingDoor))
                        {
                            TryRemoveMaterials(BarricadeMaterialCost);
                        }
                        barricadeTimeCount = 0;
                    }
                }
                else
                {
                    Game1.HUDDisplayUI.MaterialsDisplayUI.WarnInsufficientMaterials();
                }

            }
            else
            {
                barricadeTimeCount = 0;
            }
        }

        private void DoorCheck(Vector2Int playerCell)
        {
            if (!Game1.MapManager.DoorsLayer.SurroundingAreDoors(playerCell, out Vector2Int doorCell))
            {
                DeselectDoor();
                return;
            }
            SetSelectingDoor(doorCell);
        }

        private void SetSelectingDoor(Vector2Int selectingDoor)
        {
            DoorSelected = true;
            SelectingDoor = selectingDoor;
        }
        private void DeselectDoor()
        {
            DoorSelected = false;
        }

        private void CraftingUpdate()
        {
            if (!CraftingSelected)
            {
                return;
            }
            if (Input.IsRMouseFirstDown())
            {
                Game1.UpgradeWindowUI.SetActive(!Game1.UpgradeWindowUI.Active);
            }
        }

        private void CraftingCheck(Vector2Int playerCell)
        {
            if (!Game1.MapManager.CraftingLayer.SurroundingAreCrafting(playerCell, out Vector2Int craftingCell))
            {
                DeselectCrafting();
                return;
            }
            SetSelectingCrafting(craftingCell);
        }

        private void SetSelectingCrafting(Vector2Int selectingCrafting)
        {
            CraftingSelected = true;
            SelectingCrafting = selectingCrafting;
        }
        private void DeselectCrafting()
        {
            CraftingSelected = false;
            if (Game1.UpgradeWindowUI.Active)
            {
                Game1.UpgradeWindowUI.SetActive(false);
            }
        }


        #endregion

        private void CheatUpdate()
        {
            if (Input.IsKeyDown(Keys.I))
            {
                AddMaterials(100);
            }
            if (!hasDebugGun && Input.IsKeyFirstDown(Keys.O))
            {
                hasDebugGun = true;
                DebugGun debugGun = CreateWeapon(new DebugGun());
                OwnWeapon(debugGun);
            }
        }

        public override void OnCollision(DynamicCollider current, Collider other, Vector2 penetrationVector)
        {
            if (hasDied)
            {
                return;
            }
            base.OnCollision(current, other, penetrationVector);
            OnCollision_PushBack(current, other, penetrationVector);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hasDied)
            {
                return;
            }


            Texture2D bodyTexture;
            if (weapon != null)
            {
                bodyTexture = weapon.GetPlayerBodyTexture();
            }
            else
            {
                bodyTexture = this.bodyTexture;
            }
            spriteBatch.Draw(bodyTexture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder);

            spriteBatch.Draw(Texture, Position, null, Color, Rotation, OriginPixels, Scale, SpriteEffects.None, RenderOrder); //draw player head

            //if (Game1.CollisionDebugging)
            //{
            //    spriteBatch.DrawCircle((CircleF)CL.Bounds, 20, Color.Red);
            //}
            //Vector2 tl = Game1.MapManager.LocalToTileTopLeftPosition(Game1.MapManager.PositionToLocal(Position));
            //spriteBatch.DrawRectangle(new RectangleF(tl, Game1.MapManager.TileSize), Color.Blue);


            if (DoorSelected)
            {
                Vector2 pos = Game1.MapManager.LocalToTileTopLeftPosition(SelectingDoor);
                spriteBatch.Draw(UsePromptTexture, new Rectangle((int)pos.X + 16, (int)pos.Y - 32, 32, 32), Color.White);
                spriteBatch.DrawRectangle(new RectangleF(Game1.MapManager.LocalToTileTopLeftPosition(SelectingDoor), Game1.MapManager.TileSize), Color.Blue, 3);
                spriteBatch.Draw(BarricadePromptTexture, new Rectangle((int)pos.X, (int)pos.Y + 48, 64, 64), Color.White);
            }
            if (CraftingSelected)
            {
                Vector2 pos = Game1.MapManager.LocalToTileTopLeftPosition(SelectingCrafting);
                spriteBatch.Draw(UsePromptTexture, new Rectangle((int)pos.X + 16, (int)pos.Y - 32, 32, 32), Color.White);
                spriteBatch.DrawRectangle(new RectangleF(Game1.MapManager.LocalToTileTopLeftPosition(SelectingCrafting), Game1.MapManager.TileSize), Color.Yellow, 3);
            }
        }

    }
}