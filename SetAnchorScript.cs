using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class SetAnchorScript
{
    public enum MenuScreenPosition { TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left }



    /// <summary>
    /// Changes the Anchor information to match presets in the editor and adds a spacing buffer away from its outer edge, if desired
    /// </summary>
    /// <param name="activeRect"></param>
    /// <param name="targetScreenPosition"></param>
    /// <param name="buffer"></param>
    static public void SetSliderAnchorPositionsOn(RectTransform activeRect, MenuScreenPosition targetScreenPosition)
    {
        if (activeRect != null)
        {

            Vector2 parentSize = Vector2.zero;
            if (activeRect.transform.parent != null)
            {
                parentSize = activeRect.transform.parent.GetComponent<RectTransform>().sizeDelta;
            }

            switch (targetScreenPosition)
            {
                case MenuScreenPosition.Left:
                    {
                        activeRect.anchorMin = new Vector2(0, 0.5f);
                        activeRect.anchorMax = new Vector2(0, 0.5f);
                        activeRect.pivot = new Vector2(0f, 1f);
                        break;
                    }
                case MenuScreenPosition.Right:
                    {

                        activeRect.anchorMin = new Vector2(1f, 0.5f);
                        activeRect.anchorMax = new Vector2(1f, 0.5f);
                        activeRect.pivot = new Vector2(1f, 1f);
                        break;
                    }
                case MenuScreenPosition.Bottom:
                    {

                        activeRect.anchorMin = new Vector2(0.5f, 0);
                        activeRect.anchorMax = new Vector2(0.5f, 0);
                        activeRect.pivot = new Vector2(0, 0f);
                        break;
                    }
                case MenuScreenPosition.Top:
                    {

                        activeRect.anchorMin = new Vector2(0.5f, 1f);
                        activeRect.anchorMax = new Vector2(0.5f, 1f);
                        activeRect.pivot = new Vector2(0, 1f);
                        break;
                    }
            }

        }
    }

    /// <summary>
    /// Changes the Anchor information to match presets in the editor and adds a spacing buffer away from its outer edge, if desired
    /// </summary>
    /// <param name="activeRect"></param>
    /// <param name="targetScreenPosition"></param>
    /// <param name="buffer"></param>
    static public void SetAnchorPositionsOn(RectTransform activeRect, MenuScreenPosition targetScreenPosition)
    {
        SetAnchorPositionsOn(activeRect, targetScreenPosition, 0);
    }

    /// <summary>
    /// Changes the Anchor information to match presets in the editor and adds a spacing buffer away from its outer edge, if desired
    /// </summary>
    /// <param name="activeRect"></param>
    /// <param name="targetScreenPosition"></param>
    /// <param name="buffer"></param>
    static public void SetAnchorPositionsOn(RectTransform activeRect, MenuScreenPosition targetScreenPosition, float buffer)
    {
        if (activeRect != null)
        {

            Vector2 parentSize = Vector2.zero;
            if (activeRect.transform.parent != null)
            {
                parentSize = activeRect.transform.parent.GetComponent<RectTransform>().sizeDelta;
            }

            switch (targetScreenPosition)
            {
                case MenuScreenPosition.TopLeft:
                    {
                        activeRect.anchorMin = new Vector2(0, 1);
                        activeRect.anchorMax = new Vector2(0, 1);
                        activeRect.pivot = new Vector2(0f, 1f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(buffer, -buffer, 0);
                            activeRect.position = new Vector3(buffer, parentSize.y - buffer, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.Left:
                    {
                        activeRect.anchorMin = new Vector2(0, 0.5f);
                        activeRect.anchorMax = new Vector2(0, 0.5f);
                        activeRect.pivot = new Vector2(0f, 0.5f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(buffer, 0, 0);
                            activeRect.position = new Vector3(buffer, parentSize.y/2f, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.BottomLeft:
                    {

                        activeRect.anchorMin = new Vector2(0, 0);
                        activeRect.anchorMax = new Vector2(0, 0);
                        activeRect.pivot = new Vector2(0f, 0f);
                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(buffer, buffer, 0);
                            activeRect.position = new Vector3(buffer, buffer, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.Bottom:
                    {

                        activeRect.anchorMin = new Vector2(0.5f, 0);
                        activeRect.anchorMax = new Vector2(0.5f, 0);
                        activeRect.pivot = new Vector2(0.5f, 0f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(0f, buffer, 0);
                            activeRect.position = new Vector3(parentSize.x/2f,  buffer, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.BottomRight:
                    {

                        activeRect.anchorMin = new Vector2(1f, 0);
                        activeRect.anchorMax = new Vector2(1f, 0);
                        activeRect.pivot = new Vector2(1f, 0f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(-buffer, buffer, 0);
                            activeRect.position = new Vector3(parentSize.x - buffer, buffer, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.Right:
                    {

                        activeRect.anchorMin = new Vector2(1f, 0.5f);
                        activeRect.anchorMax = new Vector2(1f, 0.5f);
                        activeRect.pivot = new Vector2(1f, 0.5f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(-buffer, 0, 0);
                            activeRect.position = new Vector3(parentSize.x - buffer, parentSize.y/2f, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.TopRight:
                    {

                        activeRect.anchorMin = new Vector2(1f, 1f);
                        activeRect.anchorMax = new Vector2(1f, 1f);
                        activeRect.pivot = new Vector2(1f, 1f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(-buffer, -buffer, 0);
                            activeRect.position = new Vector3(parentSize.x - buffer, parentSize.y - buffer, 0);
                        }
                        break;
                    }
                case MenuScreenPosition.Top:
                    {

                        activeRect.anchorMin = new Vector2(0.5f, 1f);
                        activeRect.anchorMax = new Vector2(0.5f, 1f);
                        activeRect.pivot = new Vector2(0.5f, 1f);

                        if (buffer != 0)
                        {
                            //activeRect.position += new Vector3(0, -buffer, 0);
                            activeRect.position = new Vector3(parentSize.x / 2f, parentSize.y - buffer, 0);
                        }
                        break;
                    }
            }

        }
    }
}
