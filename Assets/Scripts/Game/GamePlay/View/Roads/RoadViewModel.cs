using Game.GamePlay.Services;
using Game.State.Maps.Roads;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Roads
{
    public class RoadViewModel : IMovingEntityViewModel

    {
    private readonly RoadEntity _roadEntity;
    private readonly RoadsService _service;
    public string ConfigId => _roadEntity.ConfigId;
    public bool IsTurn => _roadEntity.IsTurn;
    public ReactiveProperty<Vector2Int> Position { get; set; }
  //  public ReadOnlyReactiveProperty<Vector2Int> PointEnter { get; }
 //   public ReadOnlyReactiveProperty<Vector2Int> PointExit { get; }
    public readonly int RoadEntityId;
    public ReactiveProperty<int> Rotate { get; set; }

    public RoadViewModel(
        RoadEntity roadEntity,
        RoadsService service
    )
    {
        //TODO
        _roadEntity = roadEntity;
        _service = service;
        RoadEntityId = roadEntity.UniqueId;
        Position = roadEntity.Position;
      //  PointEnter = roadEntity.PointEnter;
     //   PointExit = roadEntity.PointExit;
        Rotate = roadEntity.Rotate;

    }

  /*  public bool IsLine()
    {
        return PointEnter.CurrentValue.x == PointExit.CurrentValue.x ||
               PointEnter.CurrentValue.y == PointExit.CurrentValue.y;
    }
*/
    public bool IsPosition(Vector2 position)
    {
        float delta = 0.5f; //Половина ширины клетки
        int _x = Position.CurrentValue.x;
        int _y = Position.CurrentValue.y;
        if ((position.x >= _x - delta && position.x <= _x + delta) &&
            (position.y >= _y - delta && position.y <= _y + delta))
            return true;
        return false;
    }

    public void SetPosition(Vector2Int position)
    {
        Position.Value = position;
    }

    public Vector2Int GetPosition()
    {
        return Position.CurrentValue;
    }
    }
}