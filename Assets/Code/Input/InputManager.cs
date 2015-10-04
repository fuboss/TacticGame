using System;
using Apex;
using Apex.Services;
using Apex.WorldGeometry;
using Assets.Code.Core;
using Assets.Code.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code.Input
{
    public class InputManager : MonoSingleton<InputManager>
    {
        /// <summary>
        /// Fired on mouse click
        /// </summary>
        public event Action<InputTarget> ClickLB;
        public event Action<InputTarget> ClickRB;


        /// <summary>
        /// Fired cancel button
        /// </summary>
        public event Action Cancel;

        /// <summary>
        /// Camera rotation
        /// </summary>
        public event Action<float> Rotation;

        public event Action<float> Zoom;

        /// <summary>
        /// Trying to switch to Next Player
        /// </summary>
        public event Action SwitchCharacters;

        //Camera control
        public event Action CameraUp;
        public event Action CameraDown;
        public event Action CameraLeft;
        public event Action CameraRight;

        /// <summary>
        /// Toggle escape menu
        /// </summary>
        public event Action EscMenu;

        public InputTarget GetInput()
        {
            UpdateInput();
            return _cachedTarget;
        }

        private Ray _inputRay;
        private InputTarget _cachedTarget;
        private EventSystem _eventSystem;
        private const float ScrollWheelGap = 0.01f;
        private const float RaycatsDistance = 100;
        private LayerMask _inputUnitLayer;
        private LayerMask _inputTerrainLayer;
        private int _cachedFrame = -1;

        private bool IsCacheValid
        {
            get { return _cachedFrame == Time.frameCount; }
        }

        private Ray ScreenToRay(Camera cam, Vector2 screenPos)
        {
            screenPos = new Vector2(Mathf.Clamp(screenPos.x, 0, cam.pixelWidth), Mathf.Clamp(screenPos.y, 0, cam.pixelHeight));
            return cam.ScreenPointToRay(screenPos);
        }

        private bool IsMouseOnScreen()
        {
            var mousePos = UnityEngine.Input.mousePosition;
            return mousePos.x > 0 && mousePos.x < Screen.width && mousePos.y > 0 &&
                                   mousePos.y < Screen.height;
        }

        private void OnRotation(float rotation)
        {
            var handler = Rotation;
            if (handler != null)
                handler(rotation);
        }

        private void DoEscMenu()
        {
            if (EscMenu != null)
                EscMenu();
        }

        private void OnCameraUp()
        {
            if (CameraUp != null)
                CameraUp();
        }

        private void OnCameraDown()
        {
            if (CameraDown != null)
                CameraDown();
        }

        private void OnCameraLeft()
        {
            if (CameraLeft != null)
                CameraLeft();
        }

        private void OnCameraRight()
        {
            if (CameraRight != null)
                CameraRight();
        }

        

        private void OnZoom(float value)
        {
            var handler = Zoom;
            if (handler != null)
                handler(value);
        }

        private void DoSwitchCharacters()
        {
            if (SwitchCharacters != null)
                SwitchCharacters();
        }

        private void ProcessHotkeys()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Tab))
                DoSwitchCharacters();

            if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
                DoEscMenu();
        }

        private void ProcessZoom()
        {
            var scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            if (Math.Abs(scroll) > ScrollWheelGap)
                OnZoom(-scroll);
        }

        private void ProcessClicks()
        {
            if(UnityEngine.Input.GetMouseButtonUp(0))
                if (ClickLB != null)
                    ClickLB(GetInput());

            if (UnityEngine.Input.GetMouseButtonUp(1))
                if (ClickRB != null)
                    ClickRB(GetInput());
        }

        private void UpdateInput()
        {
            if (IsCacheValid)  return;

            _cachedFrame = Time.frameCount;
            //over hud
            if (_eventSystem.IsPointerOverGameObject())
            {
                _cachedTarget = new InputTarget();
                return;
            }

            var input = UnityEngine.Input.mousePosition;
            var worldpos = UnityServices.mainCamera.ScreenToGroundPoint(input);
            var grid = GridManager.instance.GetGrid(worldpos);
            Character character = null;
            Cell cell = null;

            _inputRay = ScreenToRay(UnityServices.mainCamera, input);
            var allRaycastHits = Physics.RaycastAll(_inputRay, RaycatsDistance);
            Array.Sort(allRaycastHits, (x, y) => x.distance.CompareTo(y.distance));
            if (allRaycastHits.Length > 0)
            {
                foreach (var hit in allRaycastHits)
                {
                    //if ray casted to terrain
                    if (UnityServices.IsLayerInMask(hit.collider.gameObject.layer, _inputTerrainLayer))
                    {
                        cell = grid.GetCell(hit.point);
                        break;
                    }

                    if (UnityServices.IsLayerInMask(hit.collider.gameObject.layer, _inputUnitLayer))
                    {
                        character = hit.collider.GetComponentInParent<Character>();
                        break;
                    }
                }
                _cachedTarget = new InputTarget(character, cell);
            }
            else
            {
                //over hud
                _cachedTarget = new InputTarget();
            }
        }

        protected override void Awake()
        {
            _eventSystem = EventSystem.current;
            var layers = FindObjectOfType<LayerMappingComponent>();

            _inputUnitLayer = layers.unitLayer;
            _inputTerrainLayer = layers.terrainLayer;
        }

        private void Update()
        {
            if (IsMouseOnScreen())
            {
                //UpdateInput();
            }
            ProcessHotkeys();
            ProcessZoom();
            ProcessClicks();
        }
    }
}