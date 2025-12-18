using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Foxbyte.Presentation.ViewElements
{
    public class CircleToggleButton : VisualElement
    {
        private float _width;
        public float Width
        {
            get => _width;
            set
            {
                _width = value; style.width = value; _adjustIconSize();
            }
        }

        private float _height;
        public float Height
        {
            get => _height;
            set
            {
                _height = value; style.height = value; _adjustIconSize();
            }
        }

        private float _radius;
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                _setBorderRadius(value);
                _adjustIconSize();
            }
        }

        public Color DefaultColor
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                if (!_isToggled) style.backgroundColor = value;
            }
        }

        public Color ToggledColor { get; set; } = new Color(140 /255f, 205 /255f, 1f, 1f);
        
        private Color _ringColor = new Color(0f, 100 /255f, 151 /255f, 0.5f);
        public Color RingColor
        {
            get => _ringColor;
            set => _ringColor = value;
        }

        private float _ringWidth = 4;
        public float RingWidth
        {
            get => _ringWidth;
            set => _ringWidth = value;
        }
        
        private Texture2D _defaultIcon;
        public Texture2D Icon
        {
            get => _defaultIcon;
            set
            {
                _defaultIcon = value;
                if (_iconImage == null)
                {
                    _createIconImage();
                }
                if (!_isToggled)
                {
                    _iconImage.image = value;
                }
            }
        }

        private Texture2D _toggledIcon;
        public Texture2D ToggledIcon
        {
            get => _toggledIcon;
            set
            {
                _toggledIcon = value;
                if (_isToggled && _iconImage != null)
                {
                    _iconImage.image = value;
                }
            }
        }

        private float _iconSizePercentage = 100;
        public float IconSizePercentage
        {
            get => _iconSizePercentage;
            set
            {
                _iconSizePercentage = value;
                _adjustIconSize();
            }
        }

        private bool _hasConstantRing = false;
        public bool HasConstantRing
        {
            get => _hasConstantRing;
            set
            {
                _hasConstantRing = value;
                if (value)
                {
                    _showRing();
                }
                else
                {
                    _hideRing();
                }
            }
        }


        /// <summary>
        /// Flag to determine if the button should toggle its state when clicked.
        /// If false, the button will revert to its original color after being clicked after a brief moment.
        /// </summary>
        public bool IsToggle { get; set; } = true; 

        //private readonly Color _sideBarBtnColor = new Color(202 /255f, 230 /255f, 1f, 1f);
        //private readonly Color _sideBarBtnColorActiveInner = new Color(140 /255f, 205 /255f, 1f, 1f);
        //private readonly Color _sideBarBtnColorActiveRing = new Color(0f, 100 /255f, 151 /255f, 0.5f);
        private bool _isToggled = false;
        private Color _defaultColor =  new Color(202 /255f, 230 /255f, 1f, 1f);
        private Image _iconImage;
        private VisualElement _iconContainer;
        
        public CircleToggleButton()
        {
            AddToClassList("circle-button");
            style.backgroundColor = _defaultColor;
            _hideRing();

            RegisterCallback<ClickEvent>(evt =>
            {
                if (IsToggle)
                {
                    ToggleState();
                }
                else
                {
                    _showRingBriefly().Forget();
                }
            });

            _iconContainer = new VisualElement();
            Add(_iconContainer);

            // Style the icon container to fill the button but with padding
            _iconContainer.style.alignItems = Align.Center;
            _iconContainer.style.justifyContent = Justify.Center;
            _iconContainer.style.width = Length.Percent(100);
            _iconContainer.style.height = Length.Percent(100);

            // Create the icon image if it doesn't exist and add it to the container
            if (_iconImage == null)
            {
                _createIconImage();
            }
            _iconContainer.Add(_iconImage);

            // set width once so later calculations will be correct
            style.borderTopWidth = style.borderRightWidth = style.borderBottomWidth = style.borderLeftWidth = _ringWidth;
        }

        public void ToggleState()
        {
            _isToggled = !_isToggled;

            if (_isToggled)
            {
                style.backgroundColor = ToggledColor;
                _iconImage.image = _toggledIcon ? _toggledIcon : _defaultIcon;
                _showRing();
            }
            else
            {
                style.backgroundColor = _defaultColor;
                _iconImage.image = _defaultIcon;
                _hideRing();
            }
            
        }

        public void SetState(bool toggled)
        {
            _isToggled = toggled;

            if (_isToggled)
            {
                style.backgroundColor = ToggledColor;
                _iconImage.image = _toggledIcon ? _toggledIcon : _defaultIcon;
                _showRing();
            }
            else
            {
                style.backgroundColor = _defaultColor;
                _iconImage.image = _defaultIcon;
                _hideRing();
            }

        }

        private async UniTask _showRingBriefly()
        {
            _showRing();
            await UniTask.Delay(250);
            _hideRing();
            style.backgroundColor = _defaultColor;
        }

        private void _showRing()
        {
            style.borderTopColor = style.borderRightColor = style.borderBottomColor = style.borderLeftColor = _ringColor;
            //style.borderTopWidth = style.borderRightWidth = style.borderBottomWidth = style.borderLeftWidth = _ringWidth;
        }

        private void _hideRing()
        {
            if (_hasConstantRing) return;
            style.borderTopColor = style.borderRightColor = style.borderBottomColor = style.borderLeftColor = Color.clear;
        }

        private void _setBorderRadius(float value)
        {
            style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopLeftRadius = style.borderTopRightRadius = new Length(value, LengthUnit.Pixel);
        }

        private void _createIconImage()
        {
            _iconImage = new Image();
            //_iconImage.tintColor = DefaultColor;
            //Add(_iconImage); 
            //_adjustIconSize();
        }
        private void _adjustIconSize()
        {
            if (_iconImage != null)
            {
                // adjust icon size with padding
                float padding = 10f / (_iconSizePercentage / 100f);  
                _iconContainer.style.paddingTop = padding;
                _iconContainer.style.paddingBottom = padding;
                _iconContainer.style.paddingLeft = padding;
                _iconContainer.style.paddingRight = padding;
            }
        }
        public bool GetToggleStatus()
        {
            return _isToggled;
        }
    }
}