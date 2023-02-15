using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSaver
{
    public class Shape : PersistableObject
    {
        MeshRenderer meshRenderer;

        int shapeId = int.MinValue;

        public int ShapeId
        {
            get { return shapeId; }
            set 
            {
                if (shapeId == int.MinValue && value != int.MinValue)
                {
                    shapeId = value;
                }
            }
        }

        public int MaterialId
        {
            get;
            private set;
        }

        public Vector3 AngularVelocity { get; set; }

        public Vector3 Velocity { get; set; }

        Color color;
        static MaterialPropertyBlock propertyBlock;
        static int propertyColorId;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            propertyColorId = Shader.PropertyToID("_Color");
        }

        public void GameUpdate()
        {
            transform.Rotate(AngularVelocity * Time.deltaTime);
            transform.localPosition += Velocity * Time.deltaTime;
        }

        public void SetMaterial(Material mat, int matId)
        {
            meshRenderer.material = mat;
            MaterialId = matId;
        }

        public void SetColor(Color color)
        {
            this.color = color;
            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }
            propertyBlock.SetColor(propertyColorId, color);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }

        public override void Save(GameDataWritter writer)
        {
            base.Save(writer);
            writer.Write(color);
            writer.Write(AngularVelocity);
            writer.Write(Velocity);
        }

        public override void Load(GameDataReader reader)
        {
            base.Load(reader);
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
            AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
            Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        }
    }
}