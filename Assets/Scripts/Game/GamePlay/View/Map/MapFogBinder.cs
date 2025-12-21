using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.GamePlay.View.Map
{
    
    public class MapFogBinder : MonoBehaviour
    {
        [SerializeField] private Tilemap fog;

        public void Bind(MapFogViewModel viewModel)
        {
            var prefabPath = $"Prefabs/Gameplay/Maps/Fog"; //Перенести в настройки уровня
            var fogPrefab = Resources.Load<FogBinder>(prefabPath);
            var createdFog = Instantiate(fogPrefab, transform);
            
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.gameObject = createdFog.gameObject;

            
            //TileBase tile = allTiles[x + y * bounds.size.x];
            /*var b = fog.cellBounds;
            var tiles = fog.GetTilesBlock(b);
            Debug.Log(b.size); */
            //v.

            foreach (var position in viewModel.AllFogs)
            {
                fog.SetTile(new Vector3Int(position.x, position.y, 0), tile);
            }

            viewModel.AllFogs.ObserveRemove().Subscribe(e =>
            {
                fog.SetTile(new Vector3Int(e.Value.x, e.Value.y, 0), null);
            });
            createdFog.gameObject.SetActive(false);
            

        }
    }
}