using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TreeNode : Node {
    public enum State {
        Running,
        Failure,
        Success
    }


    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;

    protected OnUseData data;

    public State Run(OnUseData data) {
        started = false;
        if (!started) {
            OnStart(data);
            started = true;
        }

        state = OnUpdate();

        if (state == State.Failure || state == State.Success) {
            OnStop();
            started = false;
        }

        return state;
    }

    protected abstract void OnStart(OnUseData data);
    protected abstract void OnStop();
    protected abstract State OnUpdate();
    protected abstract string[] LogData();

    protected void CreateLogData(string[] logData) {
        data.log.LogData(logData[0], logData[1], logData[2]);
    }

}
