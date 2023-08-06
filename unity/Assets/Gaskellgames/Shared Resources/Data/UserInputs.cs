using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames
{
    [System.Serializable]
    public class UserInputs
    {
        public ButtonInputs inputButton0;
        public ButtonInputs inputButton1;
        public ButtonInputs inputButton2;
        public ButtonInputs inputButton3;
        public ButtonInputs inputButton4;
        public ButtonInputs inputButton5;
        public ButtonInputs inputButton6;
        public ButtonInputs inputButton7;
        public ButtonInputs inputButton8;
        public ButtonInputs inputButton9;

        [Range(-1, 1)] public float inputAxisX;
        [Range(-1, 1)] public float inputAxisY;
        [Range(-1, 1)] public float inputAxis4;
        [Range(-1, 1)] public float inputAxis5;
        [Range(-1, 1)] public float inputAxis6;
        [Range(-1, 1)] public float inputAxis7;
        [Range(0, 1)] public float inputAxis9;
        [Range(0, 1)] public float inputAxis10;

    } // class end
}

    
