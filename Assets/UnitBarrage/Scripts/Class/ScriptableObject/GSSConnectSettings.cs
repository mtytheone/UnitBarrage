using UnityEngine;

namespace hatuxes.UnitBarrage
{
    [CreateAssetMenu(fileName = "GSSSetting", menuName = "Settings/GSS", order = 3)]
    public class GSSConnectSettings : ScriptableObject
    {
        /// <summary>
        /// GASのURL
        /// </summary>
        public string URL;
        /// <summary>
        /// GSSのシート名
        /// </summary>
        public string SheetName;
    }
}