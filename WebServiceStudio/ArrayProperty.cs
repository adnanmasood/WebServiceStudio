using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace WebServiceStudio
{
    internal class ArrayProperty : ClassProperty
    {
        public ArrayProperty(Type[] possibleTypes, string name, Array val) : base(possibleTypes, name, val)
        {
        }

        private Array ArrayValue
        {
            get { return (base.InternalValue as Array); }
            set { base.InternalValue = value; }
        }

        [RefreshProperties(RefreshProperties.All)]
        public virtual int Length
        {
            get { return ((ArrayValue != null) ? ArrayValue.Length : 0); }
            set
            {
                int length = Length;
                int num2 = value;
                Array destinationArray = Array.CreateInstance(Type.GetElementType(), num2);
                if (ArrayValue != null)
                {
                    Array.Copy(ArrayValue, destinationArray, Math.Min(num2, length));
                }
                ArrayValue = destinationArray;
                base.TreeNode.Text = ToString();
                CreateChildren();
            }
        }

        protected override void CreateChildren()
        {
            base.TreeNode.Nodes.Clear();
            if (OkayToCreateChildren())
            {
                Type elementType = Type.GetElementType();
                int length = Length;
                for (int i = 0; i < length; i++)
                {
                    object val = ArrayValue.GetValue(i);
                    if ((val == null) && IsInput())
                    {
                        val = CreateNewInstance(elementType);
                    }
                    CreateTreeNodeProperty(base.GetIncludedTypes(elementType), base.Name + "_" + i, val)
                        .RecreateSubtree(base.TreeNode);
                }
            }
        }

        public override object ReadChildren()
        {
            Array arrayValue = ArrayValue;
            if (arrayValue == null)
            {
                return null;
            }
            int num = 0;
            for (int i = 0; i < arrayValue.Length; i++)
            {
                TreeNode node = base.TreeNode.Nodes[num++];
                var tag = node.Tag as TreeNodeProperty;
                if (tag != null)
                {
                    arrayValue.SetValue(tag.ReadChildren(), i);
                }
            }
            return arrayValue;
        }
    }
}