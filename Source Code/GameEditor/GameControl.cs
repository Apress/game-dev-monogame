using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Forms.Controls;
using PipelineExtensions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace GameEditor
{
    public class GameControl : MonoGameControl
    {
        public const string GROUND = "ground";
        public const string BUILDINGS = "buildings";
        public const string OBJECTS = "objects";
        public const string EVENTS = "events";

        public const int TILE_SIZE = 128;

        private OrthographicCamera _camera;
        private Texture2D _groundTexture;
        private Texture2D _buildingTexture;
        private Texture2D _backgroundRectangle;
        private Texture2D _eventRectangle;
        private bool _cameraDrag;
        private GameEditorTileData _draggedTile;
        private int _mouseX;
        private int _mouseY;
        private List<Level> _levels = new List<Level>();

        public Dictionary<string, TextureAtlas> Atlas { get; private set; }
        public Dictionary<string, GameObject> GameObjects { get; private set; }
        public List<GameEditorEvent> LevelEvents { get; private set; }

        public string CurrentElementName { get; set; }
        public string CurrentLayer { get; set; }
        public int CurrentLevel { get; set; }

        public event EventHandler<EventArgs> OnInitialized;
        public event EventHandler<EventSelectedArgs> OnEventSelected;
        public event EventHandler<EventArgs> OnEventDeselected;

        protected override void Initialize()
        {
            base.Initialize();

            _backgroundRectangle = new Texture2D(GraphicsDevice, 1, 1);
            _backgroundRectangle.SetData(new[] { Color.CadetBlue });
            _eventRectangle = new Texture2D(GraphicsDevice, 1, 1);
            _eventRectangle.SetData(new[] { Color.IndianRed });

            var viewportAdapter = new DefaultViewportAdapter(Editor.graphics);
            _camera = new OrthographicCamera(viewportAdapter);
            ResetCameraPosition();

            CurrentLayer = GROUND;
            CurrentLevel = 1;
            _groundTexture = Editor.Content.Load<Texture2D>("Atlas/ground");
            _buildingTexture = Editor.Content.Load<Texture2D>("Atlas/buildings");

            // load atlas
            Atlas = new Dictionary<string, TextureAtlas>();
            var groundTiles = GetGroundTiles();
            var buildingTiles = GetBuildingTiles();
            var objectTiles = GetGameObjects();

            var groundAtlas = new TextureAtlas(GROUND, _groundTexture, groundTiles);
            var buildingAtlas = new TextureAtlas(BUILDINGS, _buildingTexture, buildingTiles);

            Atlas.Add(GROUND, groundAtlas);
            Atlas.Add(BUILDINGS, buildingAtlas);

            GameObjects = GetGameObjects();
            LevelEvents = new List<GameEditorEvent>();
 
            // start with empty levels
            for (var i = 0; i < 5; i++)
            {
                var newLevel = new Level();
                _levels.Add(newLevel);
            }

            _draggedTile = null;

            OnInitialized(this, EventArgs.Empty);
        }

        public void LoadLevel()
        {
            GetCurrentLevel().Load(Editor.Content, CurrentLevel);
        }

        public void SaveCurrentLevel()
        {
            GetCurrentLevel().Save(CurrentLevel);
        }

        private Dictionary<string, Rectangle> GetGroundTiles()
        {
            return new Dictionary<string, Rectangle>
            {
                { "sand", new Rectangle(0, 0, 128, 128) },
                { "beach_tm_02_grass", new Rectangle(128, 0, 128, 128) },
                { "beach_tm_02", new Rectangle(256, 0, 128, 128) },
                { "beach_tm_01_grass", new Rectangle(384, 0, 128, 128) },
                { "beach_tm_01", new Rectangle(512, 0, 128, 128) },
                { "beach_tl_grass", new Rectangle(640, 0, 128, 128) },
                { "beach_tl", new Rectangle(768, 0, 128, 128) },
                { "beach_rm_05_grass", new Rectangle(896, 0, 128, 128) },


                { "grass", new Rectangle(0, 128, 128, 128) },
                { "beach_rm_05", new Rectangle(128, 128, 128, 128) },
                { "beach_rm_01-22", new Rectangle(256, 128, 128, 128) },
                { "beach_rm_01_grass-22", new Rectangle(384, 128, 128, 128) },
                { "beach_rm_01_grass", new Rectangle(512, 128, 128, 128) },
                { "beach_r_up_diagonal_grass", new Rectangle(768, 128, 128, 128) },
                { "beach_r_up_diagonal", new Rectangle(896, 128, 128, 128) },

                { "beach_tr_grass", new Rectangle(0, 256, 128, 128) },
                { "beach_rm_04_grass", new Rectangle(128, 256, 128, 128) },
                { "beach_r_down_diagonal_neighbour_grass", new Rectangle(256, 256, 128, 128) },
                { "beach_lm_04_grass", new Rectangle(384, 256, 128, 128) },
                { "beach_lm_04", new Rectangle(512, 256, 128, 128) },
                { "beach_lm_03_grass", new Rectangle(640, 256, 128, 128) },
                { "beach_lm_03", new Rectangle(768, 256, 128, 128) },
                { "beach_lm_02_grass", new Rectangle(896, 256, 128, 128) },

                { "beach_tr", new Rectangle(0, 384, 128, 128) },
                { "beach_rm_04", new Rectangle(128, 384, 128, 128) },
                { "beach_r_down_diagonal_neighbour", new Rectangle(256, 384, 128, 128) },
                { "beach_lm_02", new Rectangle(384, 384, 128, 128) },
                { "beach_l_up_diagonal_grass", new Rectangle(512, 384, 128, 128) },
                { "beach_l_up_diagonal", new Rectangle(640, 384, 128, 128) },
                { "beach_br_grass", new Rectangle(768, 384, 128, 128) },
                { "beach_br", new Rectangle(896, 384, 128, 128) },

                { "beach_tm_04_grass", new Rectangle(0, 512, 128, 128) },
                { "beach_rm_03_grass", new Rectangle(128, 512, 128, 128) },
                { "beach_r_down_diagonal_grass", new Rectangle(256, 512, 128, 128) },
                { "beach_lm_01_grass", new Rectangle(384, 512, 128, 128) },
                { "beach_bm_05_grass", new Rectangle(512, 512, 128, 128) },
                { "beach_bm_03_grass", new Rectangle(640, 512, 128, 128) },
                { "beach_bm_03", new Rectangle(768, 512, 128, 128) },
                { "beach_bm_02_grass", new Rectangle(896, 512, 128, 128) },

                { "beach_tm_04", new Rectangle(0, 640, 128, 128) },
                { "beach_rm_03", new Rectangle(128, 640, 128, 128) },
                { "beach_r_down_diagonal", new Rectangle(256, 640, 128, 128) },
                { "beach_lm_01", new Rectangle(384, 640, 128, 128) },
                { "beach_bm_05", new Rectangle(512, 640, 128, 128) },
                { "beach_bm_02", new Rectangle(640, 640, 128, 128) },
                { "beach_bl_grass", new Rectangle(768, 640, 128, 128) },
                { "beach_bl", new Rectangle(896, 640, 128, 128) },

                { "beach_tm_03_grass", new Rectangle(0, 768, 128, 128) },
                { "beach_rm_02_grass", new Rectangle(128, 768, 128, 128) },
                { "beach_r_diagonal_neighbour_grass", new Rectangle(256, 768, 128, 128) },
                { "beach_l_up_diagonal_neighbour_grass", new Rectangle(384, 768, 128, 128) },
                { "beach_bm_04_grass", new Rectangle(512, 768, 128, 128) },
                { "beach_bm_01_grass", new Rectangle(640, 768, 128, 128) },

                { "beach_tm_03", new Rectangle(0, 896, 128, 128) },
                { "beach_rm_02", new Rectangle(128, 896, 128, 128) },
                { "beach_r_diagonal_neighbour", new Rectangle(256, 896, 128, 128) },
                { "beach_l_up_diagonal_neighbour", new Rectangle(384, 896, 128, 128) },
                { "beach_bm_04", new Rectangle(512, 896, 128, 128) },
                { "beach_bm_01", new Rectangle(640, 896, 128, 128) },
            };
        }

        private Dictionary<string, Rectangle> GetBuildingTiles()
        {
            return new Dictionary<string, Rectangle>
            {
                { "bunker_round_1x2_bottom", new Rectangle(2, 2, 100, 230) },
                { "bunker_round_1x2_top", new Rectangle(2, 440, 74, 200) },
                { "bunker_round_2x1_bottom", new Rectangle(374, 638, 230, 100) },
                { "bunker_round_2x1_top", new Rectangle(398, 2, 200, 74) },
            };
        }

        private Dictionary<string, GameObject> GetGameObjects()
        {
            return new Dictionary<string, GameObject> 
            {
                { "Tower", new GameObject(Editor.Content.Load<Texture2D>("Sprites/Tower"), 0.3f) }
            };
        }

        private string[,] GetGroundGrid()
        {
            return GetCurrentLevel().GroundGrid;
        }

        private List<GameEditorTileData> GetBuildings()
        {
            return GetCurrentLevel().Buildings;
        }

        private List<GameEditorTileData> GetObjects()
        { 
            return GetCurrentLevel().Objects;
        }

        private List<GameEditorEvent> GetEvents()
        { 
            return GetCurrentLevel().LevelEvents;
        }

        private Level GetCurrentLevel()
        {
            return _levels[CurrentLevel - 1];
        }

        private void ResetCameraPosition()
        {
            _camera.Position = new Vector2(
                0,
                Level.LEVEL_LENGTH * TILE_SIZE - ClientSize.Height
            );
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Middle)
            {
                _cameraDrag = false;
            }
            else if (e.Button == MouseButtons.Left)
            {
                _draggedTile = null;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_cameraDrag)
            {
                _camera.Move(new Vector2(_mouseX - e.X, _mouseY - e.Y));
            }

            if (_draggedTile != null)
            {
                var deltaX = e.X - _mouseX;
                var deltaY = e.Y - _mouseY;

                _draggedTile.X += deltaX;
                _draggedTile.Y += deltaY;
            }

            _mouseX = e.X;
            _mouseY = e.Y;

            var evt = GetEventFromCoords();
            if (evt != null)
            {
                OnEventSelected(this, new EventSelectedArgs(this, evt));
            }
            else
            {
                OnEventDeselected(this, EventArgs.Empty);
            }
        }
 
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Middle)
            {
                _cameraDrag = true;
            }

            if (e.Button == MouseButtons.Left)
            {
                // find if we clicked on a tile and drag it
                var clickedTile = GetClickedTile();
                if (clickedTile != null)
                {
                    _draggedTile = clickedTile;
                }
                else
                {
                    if (CurrentLayer == EVENTS)
                    {
                        AddEvent();
                    }
                    else
                    {
                        AddTile();
                    }
                }
            }
            
            if (e.Button == MouseButtons.Right)
            {
                RemoveTile();
            }
        }

        private void RemoveTile()
        {
            if (CurrentLayer == GROUND)
            {
                var point = GetGridCoordinates();
                GetGroundGrid()[point.X, point.Y] = null;
            }
            else if (CurrentLayer == BUILDINGS)
            {
                var tile = GetAtlasTileFromCoords(BUILDINGS, GetBuildings());
                GetBuildings().Remove(tile);
            }
            else if (CurrentLayer == OBJECTS)
            {
                var tile = GetObjectTileFromCoords();
                GetObjects().Remove(tile);
            }
            else if (CurrentLayer == EVENTS)
            {
                var evt = GetEventFromCoords();
                GetEvents().Remove(evt);
            }
        }

        private GameEditorTileData GetClickedTile()
        {
            GameEditorTileData tile = null;
            if (CurrentLayer == BUILDINGS)
            {
                tile = GetAtlasTileFromCoords(BUILDINGS, GetBuildings());
            }
            else if (CurrentLayer == OBJECTS)
            {
                tile = GetObjectTileFromCoords();
            }

            return tile;
        }

        private GameEditorEvent GetEventFromCoords()
        {
            var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);

            foreach (var evt in GetEvents())
            {
                var bounds = new Rectangle(
                    -50, 
                    evt.Y,
                    1380,
                    20
                );

                if (bounds.Contains(worldCoords))
                {
                    return evt;
                }
            }

            return null;
        }

        private GameEditorTileData GetObjectTileFromCoords()
        {
            var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);

            foreach (var obj in GetObjects())
            {
                var gameObject = GameObjects[obj.Name];
                var bounds = new Rectangle(
                    obj.X, obj.Y,
                    gameObject.Width,
                    gameObject.Height
                );

                if (bounds.Contains(worldCoords))
                {
                    return obj;
                }
            }

            return null;
        }

        private GameEditorTileData GetAtlasTileFromCoords(string atlasName, List<GameEditorTileData> tileList)
        {
            var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);

            // find which object was clicked on
            foreach (var tile in tileList)
            {
                var tileLocationInWorld = new Rectangle(
                    tile.X, tile.Y, 
                    Atlas[atlasName][tile.Name].Bounds.Width,
                    Atlas[atlasName][tile.Name].Bounds.Height);

                if (tileLocationInWorld.Contains(worldCoords))
                {
                    return tile;
                }
            }

            return null;
        }

        private void AddEvent()
        {
            var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);

            var evt = GameEditorEvent.GetEvent(CurrentElementName);
            if (evt != null)
            {
                evt.Y = (int)worldCoords.Y;
                GetEvents().Add(evt);
            }
        }

        private void AddTile()
        {
            if (CurrentElementName != null && CurrentElementName.Length > 0)
            {
                if (CurrentLayer == GROUND)
                {
                    var point = GetGridCoordinates();

                    if (point.X >= 0 && point.X < Level.LEVEL_WIDTH &&
                        point.Y >= 0 && point.Y < Level.LEVEL_LENGTH)
                    {
                        GetGroundGrid()[point.X, point.Y] = CurrentElementName;
                    }
                }
                else if (CurrentLayer == BUILDINGS)
                {
                    var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);
                    GetBuildings().Add(new GameEditorTileData(CurrentElementName, (int) worldCoords.X, (int) worldCoords.Y));
                }
                else if (CurrentLayer == OBJECTS)
                {
                    var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);
                    GetObjects().Add(new GameEditorTileData(CurrentElementName, (int) worldCoords.X, (int) worldCoords.Y));
                }
            }
        }

        private Point GetGridCoordinates()
        {
            var worldCoords = _camera.ScreenToWorld(_mouseX, _mouseY);
            var gridX = (int) worldCoords.X / TILE_SIZE;
            var gridY = (int) worldCoords.Y / TILE_SIZE;

            return new Point(gridX, gridY);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw()
        {
            base.Draw();


            var transformMatrix = _camera.GetViewMatrix();

            Editor.spriteBatch.Begin(transformMatrix: transformMatrix);
            Editor.spriteBatch.Draw(
                _backgroundRectangle, 
                new Rectangle(-5, -5, TILE_SIZE * Level.LEVEL_WIDTH + 10, 
                              TILE_SIZE * Level.LEVEL_LENGTH + 10), 
                Color.White);

            DrawGround();
            DrawBuildings();
            DrawObjects();
            DrawEvents();
            Editor.spriteBatch.End();
        }

        private void DrawGround()
        {
            for (int y = 0; y < Level.LEVEL_LENGTH; y++)
            {
                for (int x = 0; x < Level.LEVEL_WIDTH; x++)
                {
                    DrawGridElement(GROUND, GetCurrentLevel().GroundGrid[x, y], x, y);
                }
            }
        }

        private void DrawBuildings()
        {
            foreach (var obj in GetBuildings())
            {
                DrawAtlasTile(_buildingTexture, BUILDINGS, obj.Name, obj.X, obj.Y);
            }
        }

        private void DrawObjects()
        {
            foreach (var obj in GetObjects())
            {
                var gameObject = GameObjects[obj.Name];
                var rectangle = new Rectangle(
                    obj.X,
                    obj.Y,
                    gameObject.Width,
                    gameObject.Height
                );

                Editor.spriteBatch.Draw(gameObject.Texture, rectangle, gameObject.Texture.Bounds, Color.White);
            }
        }

        private void DrawEvents()
        {
            foreach (var evt in GetEvents())
            {
                var rectangle = new Rectangle(
                    -50,
                    evt.Y,
                    1380,
                    20
                );

                Editor.spriteBatch.Draw(
                    _eventRectangle,
                    rectangle,
                    Color.White * 0.5f);
            }
        }

        private void DrawAtlasTile(Texture2D texture, string atlasName, string tileName, int x, int y)
        {
            if (tileName != null && tileName != "")
            {
                var rectangle = new Rectangle(
                    x,
                    y,
                    Atlas[atlasName][tileName].Width,
                    Atlas[atlasName][tileName].Height
                );
                Editor.spriteBatch.Draw(texture, rectangle, Atlas[atlasName][tileName].Bounds, Color.White);
            }
        }

        private void DrawGridElement(string atlasName, string tileName, int x, int y)
        {
            if (tileName != null && tileName != "")
            {
                var rectangle = new Rectangle(
                    x * TILE_SIZE,
                    y * TILE_SIZE,
                    TILE_SIZE,
                    TILE_SIZE
                );
                Editor.spriteBatch.Draw(_groundTexture, rectangle, Atlas[atlasName][tileName].Bounds, Color.White);
            }
        }
    }

}
