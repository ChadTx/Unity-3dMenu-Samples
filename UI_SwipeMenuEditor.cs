using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(SwipeMenuManager))]
public class UI_SwipeMenuEditor : Editor
{
    [SerializeField]
    GameObject swipeMenuObject;

    private SwipeMenuManager menuManager;
    private GameObject activeSwipeMenu;

    Color backgroundSelectedColor;

    bool showSelectionItemData = false;

    bool menuLayoutDetailsVisible = false;
    bool menuCosmeticsVisible = false;
    bool menuScrollBarsVisible = false;

    bool menuItemLayoutVisible = false;
    bool menuItemsVisible = false;

    GUIStyle swipeMenuTitleStyle = new GUIStyle();
    GUIStyle itemNameStyle = new GUIStyle();

    // Creates a UI Menu Item in GameObject/UI
    // Priority 10 ensures it is grouped with the other menu items of the same kind
    // and propagated to the hierarchy dropdown and hierarchy context menus.
    [MenuItem("GameObject/UI/SwipeMenu/Standard Menu", false, 10)]

    // This script is called on menu selection
    public static void InstantiateStandardMenu(MenuCommand menuCommand)
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            GameObjectUtility.SetParentAndAlign(eventSystem, menuCommand.context as GameObject);
        }


        // Create a custom game object
        GameObject go = new GameObject("SwipeMenu");

        Canvas swipeMenuManagerCanvas = go.AddComponent<Canvas>();
        swipeMenuManagerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasRenderer canvasRenderer = go.AddComponent<CanvasRenderer>();

        CanvasScaler canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = 1;
        canvasScaler.referencePixelsPerUnit = 100;

        GraphicRaycaster graphicRay = go.AddComponent<GraphicRaycaster>();

        go.AddComponent<SwipeMenuManager>();

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);


        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;

        go.GetComponent<SwipeMenuManager>().UpdateMenuToPresentConfiguration();
    }

    void SetTextStyles()
    {
        itemNameStyle.fontSize = 12;
        itemNameStyle.alignment = TextAnchor.MiddleLeft;
        itemNameStyle.fontStyle = FontStyle.Bold;
        itemNameStyle.normal.textColor = new Color(255, 255, 255, 255);

        swipeMenuTitleStyle.fontSize = 18;
        swipeMenuTitleStyle.alignment = TextAnchor.UpperCenter;
        swipeMenuTitleStyle.fontStyle = FontStyle.Bold;
        swipeMenuTitleStyle.normal.textColor = new Color(255, 255, 255, 255);
    }

    private void OnEnable()
    {
        SetTextStyles();

        menuManager = target as SwipeMenuManager;

        // Hide the handles of the GameObject so we don't accidentally move it instead of moving the selected item
        Tools.hidden = true;

        if (menuManager.swipeMenuType == null)
        {
            menuManager.swipeMenuType = PrefabUtility.InstantiatePrefab(swipeMenuObject) as GameObject;
            menuManager.swipeMenuType.transform.SetParent(menuManager.transform);

        }

        backgroundSelectedColor = menuManager.backgroundColor;

        menuManager.oldMenuType = menuManager.menuType;
        menuManager.oldMenuScreenPosition = menuManager.menuScreenPosition;


        UpdateMenu();
    }

    private void OnDisable()
    {
        // Unhide Tools
        Tools.hidden = false;
    }

    // Add buttons to the scripts inspector
    // Also gets called while gameObject with SwipeMenuManager is selected
    public override void OnInspectorGUI()
    {
        base.serializedObject.Update();
        GUILayout.Space(10);

        string labelHdr = "Swipe Menu Settings";
        GUILayout.Label(labelHdr, swipeMenuTitleStyle);

        GUILayout.Space(10);

        EditorGUILayout.Separator();

        menuLayoutDetailsVisible = EditorGUILayout.Foldout(menuLayoutDetailsVisible, "Menu Layout");

        if (menuLayoutDetailsVisible)
        {
            // Example for GameObject placement
            //menuManager.swipeMenuType = (GameObject)EditorGUILayout.ObjectField("MENU TEMPLATE", menuManager.swipeMenuType, typeof(GameObject), true);
            menuManager.menuTemplate = (SwipeMenuManager.MenuTemplate)EditorGUILayout.EnumPopup("- Template:", menuManager.menuTemplate);
            GUILayout.Space(10);


            menuManager.menuType = (SwipeMenuManager.MenuType)EditorGUILayout.EnumPopup("- Menu Type:", menuManager.menuType);
            base.serializedObject.FindProperty("menuArea").vector2Value = EditorGUILayout.Vector2Field("- Area:", menuManager.menuArea);
            base.serializedObject.FindProperty("shouldStretchToFillScreen").boolValue = EditorGUILayout.Toggle("- Stretch To Fill:", menuManager.shouldStretchToFillScreen);

            GUILayout.Space(10);

            menuManager.menuScreenPosition = (SetAnchorScript.MenuScreenPosition)EditorGUILayout.EnumPopup("- Screen Position:", menuManager.menuScreenPosition);
            base.serializedObject.FindProperty("bufferFromScreenEdge").floatValue = EditorGUILayout.FloatField("- Buffer From Edge: ", menuManager.bufferFromScreenEdge);

        }

        GUILayout.Space(10);

        menuCosmeticsVisible = EditorGUILayout.Foldout(menuCosmeticsVisible, "Menu Cosmetics");
        if(menuCosmeticsVisible)
        {
            base.serializedObject.FindProperty("useBackgroundColor").boolValue = EditorGUILayout.Toggle("- Use Background Color:", menuManager.useBackgroundColor);
            base.serializedObject.FindProperty("backgroundColor").colorValue = EditorGUILayout.ColorField("- Background Color:", menuManager.backgroundColor);
        }

        GUILayout.Space(10);
        menuScrollBarsVisible = EditorGUILayout.Foldout(menuScrollBarsVisible, "Menu Scrollbars");

        if(menuScrollBarsVisible)
        {
            SwipeMenuHorizontalSliderBar HSB_Component = menuManager.gameObject.GetComponent<SwipeMenuHorizontalSliderBar>();
            if (HSB_Component == null)
            {
                if (GUILayout.Button("Add Horizontal Scroll Bar"))
                {
                    SwipeMenuHorizontalSliderBar sm_HSB = menuManager.gameObject.AddComponent<SwipeMenuHorizontalSliderBar>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove Horizontal Scroll Bar"))
                {
                    SwipeMenuHorizontalSliderBar[] sliderBars = menuManager.gameObject.GetComponents<SwipeMenuHorizontalSliderBar>();
                    foreach (SwipeMenuHorizontalSliderBar swipeMenuSliderBar in sliderBars)
                    {
                        DestroyImmediate(swipeMenuSliderBar);
                    }
                }

                if (HSB_Component.needsUpdate)
                {
                    HSB_Component.needsUpdate = false;
                }
            }


            SwipeMenuVerticalSliderBar VSB_Component = menuManager.gameObject.GetComponent<SwipeMenuVerticalSliderBar>();
            if (VSB_Component == null)
            {
                if (GUILayout.Button("Add Vertical Scroll Bar"))
                {
                    SwipeMenuVerticalSliderBar sm_VSB = menuManager.gameObject.AddComponent<SwipeMenuVerticalSliderBar>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove Vertical Scroll Bar"))
                {
                    SwipeMenuVerticalSliderBar[] sliderBars = menuManager.gameObject.GetComponents<SwipeMenuVerticalSliderBar>();

                    foreach (SwipeMenuVerticalSliderBar swipeMenuSliderBar in sliderBars)
                    {
                        DestroyImmediate(swipeMenuSliderBar);
                    }
                }

                if (VSB_Component.needsUpdate)
                {
                    VSB_Component.needsUpdate = false;
                }
            }
        }

        GUILayout.Space(10);
        menuItemLayoutVisible = EditorGUILayout.Foldout(menuItemLayoutVisible, "Menu Item Layout");
        if (menuItemLayoutVisible)
        {
            base.serializedObject.FindProperty("itemSize").vector2Value = EditorGUILayout.Vector2Field("- Item Size:", menuManager.itemSize);
            base.serializedObject.FindProperty("itemSpacing").floatValue = EditorGUILayout.FloatField("- Item Spacing: ", menuManager.itemSpacing);
        }

        menuManager.activelySelectedGameObject = (GameObject)EditorGUILayout.ObjectField("Present Selection: ", menuManager.activelySelectedGameObject, typeof(GameObject), true);



        GUILayout.Space(10);
        if (GUILayout.Button("Update Menu with New Objects"))
        {
            // Tapping this button counts as a GUI.changed and will fire UpdateMenu

            if (menuManager.selectableItemsObjects.Count > 0)
            {
                for (int i = 0; i < menuManager.selectableItemsObjects.Count; i++)
                {
                    SelectableItem activeItem = menuManager.selectableItemsObjects[i];

                    if (activeItem != null)
                    {
                        string objectName = activeItem.gameObject.name;
                        GUILayout.Label(objectName, itemNameStyle);

                        if (activeItem.itemName != null)
                        {
                            activeItem.itemName = EditorGUILayout.TextField("Name: ", activeItem.itemName);
                        }


                        if (activeItem.itemDescription != null)
                        {
                            activeItem.itemDescription = EditorGUILayout.TextField("Description: ", activeItem.itemDescription);
                        }

                        //base.serializedObject.FindProperty("itemSize").vector2Value = EditorGUILayout.Vector2Field("- Item Size:", menuManager.itemSize);
                        //GUILayout
                        //EditorGUILayout.PropertyField(serializedObject.FindProperty("selectableItemsObjects[i]"));
                        GUILayout.Space(10);
                    }
                }

            }

            menuManager.PopulateMenuWithItems(menuManager.selectableGameObjects);
        }


        GUILayout.Space(10);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("selectableGameObjects"));



        // Apply Changes to GUI System
        base.serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            Debug.Log("Menu Changed! " + menuManager.selectableItemsObjects.Count);
            UpdateMenu();

        }
        base.serializedObject.ApplyModifiedProperties();
    }


    public void UpdateMenu()
    {
        Debug.Log("Update Menu");
        SwipeMenuHorizontalSliderBar sm_HSB = menuManager.gameObject.GetComponent<SwipeMenuHorizontalSliderBar>();
        SwipeMenuVerticalSliderBar sm_VSB = menuManager.gameObject.GetComponent<SwipeMenuVerticalSliderBar>();

        menuManager.activeSwipeMenuObject = menuManager.swipeMenuType;
        menuManager.UpdateMenuToPresentConfiguration();

        if (sm_HSB != null)
        {
            sm_HSB.activeSwipeMenu = menuManager.activeSwipeMenu;
            sm_HSB.scrollRect = menuManager.activeSwipeMenu.horizontalScrollRect;
            sm_HSB.scrollHandle = menuManager.activeSwipeMenu.horizontalScrollHandle;
        }

        if (sm_VSB != null)
        {
            sm_VSB.activeSwipeMenu = menuManager.activeSwipeMenu;
            sm_VSB.scrollRect = menuManager.activeSwipeMenu.verticalScrollRect;
            sm_VSB.scrollHandle = menuManager.activeSwipeMenu.verticalScrollHandle;
        }


        menuManager.UpdateDisplayBars(sm_HSB, sm_VSB);

        MarkSceneAsDirty();
    }

    // Marks scene as dirty, to get Unity editor to save changes to GameObjects when save happens
    void MarkSceneAsDirty()
    {
        if (Application.isPlaying)
        {
            return;
        }
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }
}
