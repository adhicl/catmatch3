using System;
using UnityEngine;

namespace UI
{
    public class RowCatPickButtons : MonoBehaviour
    {
        [SerializeField] CatPickButton[] catPickButtons;

        public CatPickButton[] GetCatPickButtons()
        {
            return catPickButtons;
        }
    }
}