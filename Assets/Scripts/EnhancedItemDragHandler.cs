using UnityEngine;
using UnityEngine.EventSystems;

    [RequireComponent(typeof(CanvasGroup))] // To make sure the elemement is there, safety check
    public class EnhancedItemDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected EnhancedItemSlotUI itemSlotUI = null;
        //[SerializeField] protected HotbarItemEvent onMouseStartHoverItem = null; 
        //[SerializeField] protected VoidEvent onMouseEndHoverItem = null;

        private CanvasGroup canvasGroup = null; // 
        private Transform originalParent = null; // When we drag an slot and we release, we want to drag back to its original parent
        private bool isHovering = false;

        public EnhancedItemSlotUI ItemSlotUI => itemSlotUI; // Getter, because we may want to reference itemSlotUI in other classes.

    private void Start() { 

        canvasGroup = GetComponent<CanvasGroup>(); 
    }

        private void OnDisable()
        {
            // If we're hovering the slot:
            if (isHovering)
            {
                //onMouseEndHoverItem.Raise();
                isHovering = false;
            }
        }
        
        // Button press:
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // If the button we're pressing is the left mouse button, then:
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //onMouseEndHoverItem.Raise();

                originalParent = transform.parent;

                transform.SetParent(transform.parent.parent); // Setting the parent to be the 2x up from this.

                canvasGroup.blocksRaycasts = false; // When dragging the item around into another slot, we want to ignore the item we're dragging and look what's underneath it (so it doesn't think that for example you have dropped on itself, but on the item below it)
            }
        }
        

        public virtual void OnDrag(PointerEventData eventData)
        {
            // If the button we're pressing is the left mouse button, then:
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.position = Input.mousePosition; // If we're dragging to a lot, we'll set the position whenever our pointer is
            }
        }
        
        // Buton release:
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.SetParent(originalParent);
                transform.localPosition = Vector3.zero;
                canvasGroup.blocksRaycasts = true; // Back to what it was at the start
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //onMouseStartHoverItem.Raise(ItemSlotUI.EnhancedSlotItem);
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //onMouseEndHoverItem.Raise();
            isHovering = false;
        }
    }
