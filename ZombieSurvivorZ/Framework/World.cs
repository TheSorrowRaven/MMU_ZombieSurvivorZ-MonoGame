using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{

    // This class manages the second phase of Game Loop

    // Limitations:
    // The world does not check for duplicate GameObject entries. So don't add the same object multiple times :)

    public class World
    {
        public static readonly World Current = new();

        // Currently active GameObjects are in this list
        private readonly List<GameObject> gameObjects = new();

        // When new GameObjects are created, they are added to this list first.
        private readonly List<GameObject> newObjects = new();

        // When GameObjects call Destroy(), they are added to this list first.
        private readonly List<GameObject> flaggedObjects = new();

        // Generic helper method to create a specific GameObject and
        // add it to the world in one line
        // Usage:
        // T go = World.CreateGameObject<T>("objectName");
        public static T CreateGameObject<T>(string name) where T : GameObject
        {
            T go = (T)Activator.CreateInstance(typeof(T));
            go.Name = name;

            AddGameObject(go);
            return go;
        }

        // For GameObjects created with new operator,
        // need to add to World manually.
        // E.g.
        // ConcreteGameObject go = new ConcreteGameObject("foo");
        // World.AddGameObject(go);
        public static void AddGameObject(GameObject go)
        {
            Current.newObjects.Add(go);
        }

        // Called by GameObject.Destroy
        public static void RemoveGameObject(GameObject go)
        {
            Current.flaggedObjects.Add(go);
        }

        public static GameObject FindGameObject(string name)
        {
            return Current.gameObjects.Find(x => x.Name.Equals(name));
        }

        public static GameObject FindGameObject(GameObject go)
        {
            return Current.gameObjects.Find(x => x.Equals(go));
        }

        public static void Clear()
        {
            Current.gameObjects.Clear();
        }

        public static void Update(GameTime gameTime)
        {
            var worldGameObjects = Current.gameObjects;

            // Remove GameObjects that are marked for deletion
            var list = new List<GameObject>(Current.flaggedObjects);
            // Clear flaggedObjects for next frame
            Current.flaggedObjects.Clear();
            if (list.Count > 0)
            {
                foreach (var entity in list)
                {
                    entity.OnDestroy();
                    worldGameObjects.Remove(entity);
                }
            }

            // Initialize newly added GameObjects
            // Then add them to gameObjects list
            list = new List<GameObject>(Current.newObjects);
            // Clear newObjects for next frame
            Current.newObjects.Clear();
            if (list.Count > 0)
            {
                foreach (var entity in list)
                {
                    entity.Initialize();
                    worldGameObjects.Add(entity);
                }
            }

            // Update all active GameObjects
            foreach (var entity in worldGameObjects)
            {
                entity.Update();
            }
        }

        public static void Draw(SpriteBatch batch, GameTime gameTime)
        {
            foreach (GameObject entity in Current.gameObjects)
            {
                if (!entity.Active)
                {
                    continue;
                }
                entity.Draw(batch);
            }
        }
    }
}