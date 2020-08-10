using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiddleGround.UI.DiceAnimation
{
    public class MG_GamePanel_DiceAnimation : MonoBehaviour
    {
        MG_GamePanel_Dice parent;
        private void Awake()
        {
            parent = GetComponentInParent<MG_GamePanel_Dice>();
        }
        public void RollEnd()
        {
            parent.RollEnd();
        }
    }
}
