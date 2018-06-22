using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public class Panels : MonoBehaviour
    {
        private Dictionary<string, Menu> menusDic;

        private void Start()
        {
            Menu[] menus = GetComponentsInChildren<Menu>();
            menusDic = new Dictionary<string, Menu>();
            foreach (Menu menu in menus)
            {
                menusDic[menu.name] = menu;
            }
        }

        public void ShowPanel(string name, bool hidePreview)
        {
            if (hidePreview)
            {
                foreach (Menu menu in menusDic.Values)
                {
                    menu.Hide();
                }
                menusDic[name].Show();
            }
        }
    }
}