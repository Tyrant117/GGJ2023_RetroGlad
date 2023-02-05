using UnityEngine;

namespace RetroGlad
{
    public class FloatingEquipment : MonoBehaviour
    {
        [SerializeField]
        private Transform _owner;
        [SerializeField]
        private Entity _entity;
        [SerializeField]
        private float _offset;

        [SerializeField]
        private bool _flip;
        [SerializeField]
        private bool _directionIsFacing;

        [SerializeField]
        private Transform _ownerBehind;
        [SerializeField]
        private Transform _flipWeapon;

        private SpriteRenderer _renderer;
        private Transform _behind;

        public Vector3 Direction { get; private set; }
        public bool Frozen { get; set; }

        private void OnValidate()
        {
            _owner = transform.parent;            
            _ownerBehind = transform.parent.Find("Behind");
            _flipWeapon = transform.parent.Find("Flip Weapon");
        }

        private void Awake()
        {
            _entity = GetComponentInParent<Entity>();
            _renderer = GetComponent<SpriteRenderer>();
            _behind = transform.Find("Behind");
        }

        private void Update()
        {
            if (Frozen) { return; }

            if (_entity.IsNPC)
            {
                var dir = _entity.Motor.Direction;
                Direction = dir;
                transform.position = _owner.transform.position + dir * _offset;
            }
            else
            {
                var mwp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var target = new Vector3(mwp.x, mwp.y, 0);
                var dir = (target - _owner.position).normalized;
                Direction = dir;
                transform.position = _owner.transform.position + dir * _offset;
            }

            if(_behind.position.y > _ownerBehind.position.y)
            {
                _renderer.sortingOrder = -1;
            }
            else
            {
                _renderer.sortingOrder = 0;
            }

            if (_flip)
            {
                if (_behind.position.y > _flipWeapon.position.y)
                {
                    _renderer.flipY = false;
                }
                else
                {
                    _renderer.flipY = true;
                }
            }

            if (_directionIsFacing)
            {
                transform.right = Direction;
            }
        }
    }
}
