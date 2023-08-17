using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;

    Vector3 positonFrom, positionTo;

    float progress;

    Direction direction;

    DirectionChange directionChange;

    float directionAngleFrom, directionAngleTo;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory");
            originFactory = value;
        }
    }
   
    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextOnPath != null, "Nowhere to go!");
        tileFrom = tile;
        tileTo = tile.NextOnPath;
        progress = 0f;
        PreparIntro();
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime;
        while(progress >= 1f)
        {
            tileFrom = tileTo;
            tileTo = tileTo.NextOnPath;
            if(tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            progress -= 1f;
            PreparNextState();
        }

        transform.localPosition = Vector3.LerpUnclamped(positonFrom, positionTo, progress);
        if(directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        return true;
    }

    void PreparIntro()
    {
        positonFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
    }

    void PreparNextState()
    {
        positonFrom = positionTo;
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectioChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;
        switch (directionChange)
        {
            case DirectionChange.TurnRight: PreparTurnRight(); break;
            case DirectionChange.TurnLeft: PreparTurnLeft(); break;
            case DirectionChange.TurnAround: PreparTurnAround(); break;
            default:PreparForward(); break;
        }
    }

    void PreparForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
    }

    void PreparTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
    }

    void PreparTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
    }

    void PreparTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180f;
    }
}
