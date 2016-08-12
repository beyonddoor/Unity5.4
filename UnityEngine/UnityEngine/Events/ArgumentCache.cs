namespace UnityEngine.Events
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    internal class ArgumentCache : ISerializationCallbackReceiver
    {
        [SerializeField]
        private bool m_BoolArgument;
        [FormerlySerializedAs("floatArgument"), SerializeField]
        private float m_FloatArgument;
        [SerializeField, FormerlySerializedAs("intArgument")]
        private int m_IntArgument;
        [SerializeField, FormerlySerializedAs("objectArgument")]
        private UnityEngine.Object m_ObjectArgument;
        [SerializeField, FormerlySerializedAs("objectArgumentAssemblyTypeName")]
        private string m_ObjectArgumentAssemblyTypeName;
        [FormerlySerializedAs("stringArgument"), SerializeField]
        private string m_StringArgument;

        public void OnAfterDeserialize()
        {
            this.TidyAssemblyTypeName();
        }

        public void OnBeforeSerialize()
        {
            this.TidyAssemblyTypeName();
        }

        private void TidyAssemblyTypeName()
        {
            if (!string.IsNullOrEmpty(this.m_ObjectArgumentAssemblyTypeName))
            {
                int num = 0x7fffffff;
                int index = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", Version=");
                if (index != -1)
                {
                    num = Math.Min(index, num);
                }
                index = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", Culture=");
                if (index != -1)
                {
                    num = Math.Min(index, num);
                }
                index = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", PublicKeyToken=");
                if (index != -1)
                {
                    num = Math.Min(index, num);
                }
                if (num != 0x7fffffff)
                {
                    this.m_ObjectArgumentAssemblyTypeName = this.m_ObjectArgumentAssemblyTypeName.Substring(0, num);
                }
            }
        }

        public bool boolArgument
        {
            get
            {
                return this.m_BoolArgument;
            }
            set
            {
                this.m_BoolArgument = value;
            }
        }

        public float floatArgument
        {
            get
            {
                return this.m_FloatArgument;
            }
            set
            {
                this.m_FloatArgument = value;
            }
        }

        public int intArgument
        {
            get
            {
                return this.m_IntArgument;
            }
            set
            {
                this.m_IntArgument = value;
            }
        }

        public string stringArgument
        {
            get
            {
                return this.m_StringArgument;
            }
            set
            {
                this.m_StringArgument = value;
            }
        }

        public UnityEngine.Object unityObjectArgument
        {
            get
            {
                return this.m_ObjectArgument;
            }
            set
            {
                this.m_ObjectArgument = value;
                this.m_ObjectArgumentAssemblyTypeName = (value == null) ? string.Empty : value.GetType().AssemblyQualifiedName;
            }
        }

        public string unityObjectArgumentAssemblyTypeName
        {
            get
            {
                return this.m_ObjectArgumentAssemblyTypeName;
            }
        }
    }
}

