using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHelp : MonoBehaviour
{

    PlayerMove playerMove;
    PlayerAttack playerAttack;

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }
    
    //Move
    public void Roll() { playerMove.PlayRoll(); }

    public void FinishRoll() { playerMove.FinishedRoll(); }

    //Attack
    public void BackLayer() {  playerAttack.BackToLayerBase(); } 

    public void Shoot() { playerAttack.Disparar(); }
}
