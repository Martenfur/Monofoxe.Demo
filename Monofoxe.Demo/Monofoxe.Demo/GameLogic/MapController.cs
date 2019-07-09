using System.Collections.Generic;
using Monofoxe.Tiled;
using Monofoxe.Demo.GameLogic.Entities;

namespace Monofoxe.Demo.GameLogic
{
	public static class MapController
	{
	
		
		private static List<MapBuilder> _mapList;

		private static int _currentMapIndex = 0;

		public static MapBuilder CurrentMap => _mapList[_currentMapIndex];

		public static void Init()
		{
			_mapList = new List<MapBuilder>();

			_mapList.Add(new ColliderMapBuilder(Resources.Maps.Level1));
			_mapList.Add(new ColliderMapBuilder(Resources.Maps.Level2));
		}

		public static void BuildNextMap()
		{
			CurrentMap.Destroy();
			_currentMapIndex += 1;
			if (_currentMapIndex >= _mapList.Count)
			{
				_currentMapIndex = 0;
				
			}
			CurrentMap.Build();
			if (_currentMapIndex == 0)
			{
				new TitleScreen(GameplayController.GUILayer);
			}
		}

		public static void BuildPreviousMap()
		{
			CurrentMap.Destroy();
			_currentMapIndex -= 1;
			if (_currentMapIndex < 0)
			{
				_currentMapIndex = _mapList.Count - 1;
			}
			CurrentMap.Build();
		}

		public static void RebuildCurrentMap()
		{
			CurrentMap.Destroy();
			CurrentMap.Build();
		}


	}
}
