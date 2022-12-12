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

        public static readonly World floor = new();
        public static readonly World objects = new();
        public static readonly World UI = new();



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
        public T CreateGameObject<T>(string name) where T : GameObject
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
        public void AddGameObject(GameObject go)
        {
            newObjects.Add(go);
        }

        // Called by GameObject.Destroy
        public void RemoveGameObject(GameObject go)
        {
            flaggedObjects.Add(go);
        }

        public GameObject FindGameObject(string name)
        {
            return gameObjects.Find(x => x.Name.Equals(name));
        }

        public GameObject FindGameObject(GameObject go)
        {
            return gameObjects.Find(x => x.Equals(go));
        }

        public void Clear()
        {
            gameObjects.Clear();
        }

        public void Update(GameTime gameTime)
        {
            var worldGameObjects = gameObjects;

            // Remove GameObjects that are marked for deletion
            var list = new List<GameObject>(flaggedObjects);
            // Clear flaggedObjects for next frame
            flaggedObjects.Clear();
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
            list = new List<GameObject>(newObjects);
            // Clear newObjects for next frame
            newObjects.Clear();
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

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            foreach (GameObject entity in gameObjects)
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