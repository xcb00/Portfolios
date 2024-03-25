using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : PlayerCursor
{
    public void PlayerAct()
    {
        bool playAnimation = false;

        if(myAction.currentTool==PlayerTool.Interactor)
            EventHandler.CallOpenPanelEvent();

        if (!canAction)
            return;

        if (myAnimator.GetBool(EnumCaching.ToString(AnimatorParameters.playing)))
        {
            Debug.Log("애니메이션 재생 중");
            return;
        }

        switch (myAction.currentTool)
        {
            case PlayerTool.Hoe:
                playAnimation = SystemTileManager.Instance.CreateDugTile(PlayerCursorCoordinate());
                break;
            case PlayerTool.Water:
                playAnimation = SystemTileManager.Instance.CreateWaterTile(PlayerCursorCoordinate());
                break;
            case PlayerTool.Seed:
                // 씨앗을 선택했는지 확인
                //playAnimation = seedCode>0 && CropManager.Instance.SpawnCrop(PlayerCursorCoordinate(), seedCode);
                if (seedCode < 0) playAnimation = false;
                else
                {
                    if (GameDatas.itemDetailsList.Find(x => x.code == seedCode).type == ItemType.seed)
                        playAnimation = CropManager.Instance.SpawnCrop(PlayerCursorCoordinate(), seedCode);
                    else if (GameDatas.itemDetailsList.Find(x => x.code == seedCode).type == ItemType.fruit)
                    {
                        playAnimation = TreeManager.Instance.SpawnTree(PlayerCursorCoordinate(), seedCode);
                    }
                    else
                        playAnimation = false;
                }
                break;
            case PlayerTool.Axe:
                //playAnimation = ScaneryManager.Instance.HitScanery(PlayerCursorCoordinate(), ScaneryType.tree);
                TreeManager.Instance.HitTree(PlayerCursorCoordinate());
                playAnimation = true;
                break;
            case PlayerTool.PickAxe:
                //playAnimation = O_ScaneryManager.Instance.HitScanery(PlayerCursorCoordinate());
                OreManager.Instance.HitOre(PlayerCursorCoordinate());
                playAnimation = true;
                break;
            case PlayerTool.Pickup:
                playAnimation = CropManager.Instance.Harvest(PlayerCursorCoordinate());
                break;
            case PlayerTool.Attack:
                playAnimation = true;
                break;
            case PlayerTool.Fish:
                if (!isFishing)
                {
                    isFishing = true;
                    myAnimator.SetTrigger(EnumCaching.ToString(myAction.currentTool));

                }
                else
                {
                    isFishing = false;
                }

                myAnimator.SetBool(EnumCaching.ToString(AnimatorParameters.isFishing), isFishing);
                EventHandler.CallOpenFishPanel(isFishing, fishType);
                break;/*
            case PlayerTool.Interactor:
                EventHandler.CallOpenPanelEvent();
                break;*/
            default:
                break;
        }

        if (playAnimation)
        {
            myAnimator.SetTrigger(EnumCaching.ToString(myAction.currentTool));
            CheckTile();
        }
    }
}
