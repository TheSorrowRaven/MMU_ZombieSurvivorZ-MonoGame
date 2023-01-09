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

        private float movementSpeed = 250;
        private bool sameKeyHolstersWeapon = true;

        private Reticle reticle;
        private Weapon weapon;

        private Texture2D bodyTexture;

        private readonly List<Weapon> weapons = new();
        private readonly Dictionary<int, Weapon> keyNumToWeapon = new();
        private int keyNumCount = 1;
        public int Materials { get; private set; }


        private Vector2Int lastCellPos;

        public bool DoorSelected = false;
        public Vector2Int SelectingDoor;

        public bool CraftingSelected = false;
        public Vector2Int SelectingCrafting;


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
            CL = new CircleDynamicCollider(this, 25);
        }

        public override void Initialize()
        {
            Texture = Game1.GetTexture("Player/player_head");
            bodyTexture = Game1.GetTexture("Player/player_body");

            RotationOffset = 90 * MathF.PI / 180;
            Position = new(0, 0);

            //Scale = new(0.5f, 0.5f);

            Pistol pistol = CreateWeapon(new Pistol());
            OwnWeapon(pistol);

            DebugGun debugGun = CreateWeapon(new DebugGun());
            OwnWeapon(debugGun);

            AddWeaponsToWeaponUpgradeUI();

            reticle = new();
            reticle.Disable();

            AddMaterials(100);  //Initial Materials
        }

        protected override void ScaleChanged()
        {
            base.ScaleChanged();
            ((CircleDynamicCollider)CL).Set(25 * Scale.X);
            if (weapon != null)
            {
                weapon.Scale = Scale;
            }
        }

        public override void Update()
        {
            TransformUpdate();
            WeaponOperationUpdate();
            WeaponUpdate();
            InteractionUpdate();
        }

        private void TransformUpdate()
        {
            Vector2 movement = Input.ConstructAxis2(Keys.S, Keys.W, Keys.D, Keys.A);
            Position += movement * movementSpeed * Time.deltaTime;
            Heading = Game1.Camera.ScreenToWorld(reticle.Position) - Position;

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
            if (sameKeyHolstersWeapon)
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
            }

            //Updates
            DoorUpdate();
            CraftingUpdate();
        }

        private void DoorUpdate()
        {
            if (!DoorSelected)
            {
                return;
            }
            if (Input.IsKeyFirstDown(Keys.F))
            {
                Game1.MapManager.DoorsLayer.ToggleDoor(SelectingDoor);
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
            if (Input.IsKeyFirstDown(Keys.F))
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



        #region External




        #endregion

        public override void OnCollision(DynamicCollider current, Collider other, Vector2 penetrationVector)
        {
            base.OnCollision(current, other, penetrationVector);
            OnCollision_PushBack(current, other, penetrationVector);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
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

            if (Game1.CollisionDebugging)
            {
                spriteBatch.DrawCircle((CircleF)CL.Bounds, 20, Color.Red);
            }
            //Vector2 tl = Game1.MapManager.LocalToTileTopLeftPosition(Game1.MapManager.PositionToLocal(Position));
            //spriteBatch.DrawRectangle(new RectangleF(tl, Game1.MapManager.TileSize), Color.Blue);


            if (DoorSelected)
            {
                spriteBatch.DrawRectangle(new RectangleF(Game1.MapManager.LocalToTileTopLeftPosition(SelectingDoor), Game1.MapManager.TileSize), Color.Blue, 3);
            }
            if (CraftingSelected)
            {
                spriteBatch.DrawRectangle(new RectangleF(Game1.MapManager.LocalToTileTopLeftPosition(SelectingCrafting), Game1.MapManager.TileSize), Color.Red, 3);
            }
        }

    }
}