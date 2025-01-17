﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Console
{
    public abstract class ConsoleCommand
    {
        public abstract string Name { get; protected set; }

        public abstract string Command { get; protected set; }

        public abstract string Description { get; protected set; }

        public abstract string Help { get; protected set; }

        public void AddCommandToConsole()
        {
            string addMessage = " command has been added to the console.";
            DeveloperConsole.AddCommandsToConsole(Command, this);
            Debug.Log(Name + addMessage);
        }

        public abstract void RunCommand();
    }
    public class DeveloperConsole : MonoBehaviour
    {
        
        public static DeveloperConsole Instance { get; private set; } //singleton this
        public static Dictionary<string,ConsoleCommand> Commands { get; private set; }

        [Header("UI Components")]
        public Canvas consoleCanvas;
        public Text consoleText;
        public Text inputText;
        public InputField consoleInput;

        private void Awake()
        {
            if(Instance != null)
            {
                return;
            }

            Instance = this;
            Commands = new Dictionary<string, ConsoleCommand>();
        }

        private void Start()
        {
            consoleCanvas.gameObject.SetActive(false);
            CreateCommands();
        }
        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog; 
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string _message = "[" + type.ToString() + "] " + logMessage; //custom debug log hook message
            AddMessageToConsole(_message);
        }
        private void CreateCommands()
        {
            CommandQuit.CreateCommand();
        }

        public static void AddCommandsToConsole( string _name, ConsoleCommand _command)
        {
            if (!Commands.ContainsKey(_name))
            {
                Commands.Add(_name, _command);
            }

        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy); //checks if its active in hierarchy and enables or disables
                if (consoleCanvas.isActiveAndEnabled)
                {
                    consoleInput.ActivateInputField(); //enables focus into the console.
                }
            }

            if (consoleCanvas.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(inputText.text != "")
                    {
                        AddMessageToConsole(inputText.text);
                        ParseInput(inputText.text);
                        //inputText.text = ""; //resets the input
                        consoleInput.text = "";
                        consoleInput.ActivateInputField();
                    }
                }
            }
        }

        private void AddMessageToConsole(string msg)
        {
            consoleText.text += msg + "\n";
        }

        private void ParseInput(string input)
        {
            string[] _input = input.Split(null); //separate 

            if(_input.Length == 0 || _input == null)
            {
                //AddMessageToConsole("Command not recognized.");
                Debug.LogWarning("Command not recognized");
                return;
            }

            if (!Commands.ContainsKey(_input[0]))
            {
                Debug.LogWarning("Command not recognized.");
                //Debug.LogError
            }
            else
            {
                Commands[_input[0]].RunCommand();//TODO ADD ARGUMENTS
            }
        }
    }
}
