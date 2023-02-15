using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSaver
{
    [CreateAssetMenu()]
    public class ShapeFactory : ScriptableObject
    {
        [SerializeField]
        List<Shape> shapes;

        [SerializeField]
        List<Material> mats;

        [SerializeField]
        bool recycle;

        List<Shape>[] pools;

        Scene poolScene;

        public Shape Get(int shapeId = 0, int matId = 0)
        {
            Shape shape;
            if (recycle)
            {
                if(pools == null)
                {
                    CreatePools();
                }

                int count = pools[shapeId].Count;
                if (count > 0)
                {
                    shape = pools[shapeId][count - 1];
                    shape.gameObject.SetActive(true);
                    pools[shapeId].RemoveAt(count - 1);
                }
                else
                {
                    shape = Instantiate(shapes[shapeId]);
                    SceneManager.MoveGameObjectToScene(shape.gameObject, poolScene);
                }
            }
            else
            {
                shape = Instantiate(shapes[shapeId]);
                SceneManager.MoveGameObjectToScene(shape.gameObject, poolScene);
            }

            shape.ShapeId = shapeId;
            shape.SetMaterial(mats[matId], matId);
            return shape;
        }

        public Shape GetRandom()
        {
            int shapeId = Random.Range(0, shapes.Count);
            int matId = Random.Range(0, mats.Count);
            Shape shape = Get(shapeId, matId);
            return shape;
        }

        public void Reclaim(Shape shapeToReclaim)
        {
            if (recycle)
            {
                if(pools == null)
                {
                    CreatePools();
                }

                pools[shapeToReclaim.ShapeId].Add(shapeToReclaim);
                shapeToReclaim.gameObject.SetActive(false);
            }
            else
            {
                Destroy(shapeToReclaim.gameObject);
            }
        }

        void CreatePools()
        {
            pools = new List<Shape>[shapes.Count];
            for(int i = 0;i < shapes.Count; i++)
            {
                pools[i] = new List<Shape>();
            }

#if UNITY_EDITOR
            poolScene = SceneManager.GetSceneByName(name);
            if (poolScene.isLoaded)
            {
                GameObject[] objs = poolScene.GetRootGameObjects();
                foreach(var obj in objs)
                {
                    Shape shape = obj.GetComponent<Shape>();
                    if (shape && obj.activeInHierarchy)
                    {
                        pools[shape.ShapeId].Add(shape);
                    }
                }

                return;
            }
#endif

            poolScene = SceneManager.CreateScene(name);
        }
    }
}
