using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    public class FolderPathAttributeDrawer : EasyAttributeDrawer<FolderPathAttribute>
    {
        protected override void Draw(GUIContent label)
        {
            //TODO FolderPathAttributeDrawer
            CallNextDrawer(label);
        }
    }
}
