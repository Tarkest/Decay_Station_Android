using UnityEditor;
using static UnityEditor.EditorGUILayout;
using UnityEngine;

public class EDITOR_ServerData : EditorWindow
{
    #region Render Variables

    private GUIContent[] _icons = new GUIContent[2];
    private Entities _currentTab;
    private Entities _prevTab = Entities.Empty;
    private enum Entities
    {
        Empty = -1,
        Constants,
        Items,
    }

    private string _errorMessage = "";
    private bool _isError = false;

    #endregion

    #region Views

    private EDITOR_ConstantView _constantsView;
    private EDITOR_ItemsView _itemsView;

    #endregion

    #region Data Buffer

    #region Misc

    private static EDITOR_ServerData _window;

    #endregion

    #region Login

    private bool _isLoggedIn = false;
    private string _token = "";
    private string _loginBuffer = "";
    private string _passwordBuffer = "";

    #endregion


    #endregion

    [MenuItem("Editors/Serverdata")]
    public static void ShowLocomotiveEditorWindow()
    {
        _window = GetWindow<EDITOR_ServerData>("Server Data");
        _window.LoadResouces();
    }

    public void LoadResouces()
    {
        _icons[0] = new GUIContent((Texture)EditorGUIUtility.Load("icons/constants.png"), "Constants");
        _icons[1] = new GUIContent((Texture)EditorGUIUtility.Load("icons/item.png"), "Items");
        //_icons[1] = new GUIContent((Texture)EditorGUIUtility.Load("icons/carriage.png"), "Carriages");
        //_icons[3] = new GUIContent((Texture)EditorGUIUtility.Load("icons/building.png"), "Buildings");
        //_icons[4] = new GUIContent((Texture)EditorGUIUtility.Load("icons/recipe.png"), "Recipes");
        //_icons[5] = new GUIContent((Texture)EditorGUIUtility.Load("icons/map.png"), "Map");
        //_icons[6] = new GUIContent((Texture)EditorGUIUtility.Load("icons/constants.png"), "Constants");
    }

    void OnGUI()
    {
        if(_isError)
        {
            RenderError();
        }
        else
        {
            if (_isLoggedIn)
            {
                _currentTab = (Entities)GUILayout.Toolbar((int)_currentTab, _icons);
                RenderView();
                _prevTab = _currentTab;
            }
            else
            {
                RenderLogin();
            }
        }
    }

    #region Components

    private void RenderError()
    {
        GUILayout.FlexibleSpace();
        BeginVertical("box");
        HelpBox(_errorMessage, MessageType.Error);
        if (GUILayout.Button("Ok"))
        {
            _errorMessage = "";
            _isError = false;
        }
        EndVertical();
        GUILayout.FlexibleSpace();
    }

    private void RenderLogin()
    {
        GUILayout.FlexibleSpace();
        BeginVertical("box");
        LabelField("Admin login");
        _loginBuffer = TextField(_loginBuffer);
        LabelField("Admin password");
        _passwordBuffer = PasswordField(_passwordBuffer);
        Space();
        if(GUILayout.Button("Login"))
        {
            Login(_loginBuffer, _passwordBuffer);
        }
        if(GUILayout.Button("Cancel"))
        {
            _window.Close();
        }
        EndVertical();
        GUILayout.FlexibleSpace();
    }

    private void RenderView()
    {
        switch(_currentTab)
        {
            case Entities.Constants:
                if (_constantsView == null)
                {
                    _constantsView = new EDITOR_ConstantView(_window, _token);
                    _itemsView = null;
                }
                _constantsView.ConstantsTypesView();
                break;

            case Entities.Items:
                if(_itemsView == null)
                {
                    _constantsView = null;
                    _itemsView = new EDITOR_ItemsView(_window, _token);
                }
                _itemsView.ItemsView();
                break;
        }
    }

    #endregion

    #region Network Methods

    private void Login(string login, string password)
    {
        EDITOR_Utility.POST("login", JSON.ToJSON(new AccountData(login, password)), LoginCallback, _token);
    }

    #endregion

    #region Network CallBacks

    private void LoginCallback(string data, string error)
    {
        _window.Focus();
        if(error == null)
        {
            _loginBuffer = "";
            _passwordBuffer = "";
            _token = data;
            _isLoggedIn = true;
        }
        else
        {
            _errorMessage = error;
            _isError = true;
        }
    }

    #endregion

    #region Misc Methods

    public void SetError(string error)
    {
        _errorMessage = error;
        _isError = true;
    }

    #endregion
}
