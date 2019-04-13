using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween : MonoBehaviour {

    public static Tween a;
    public List<Task> tasks;
    public List<Task> tasksToDelete;

    void Awake () {
        a = this;
        tasks = new List<Task>();
    }
	
	void FixedUpdate () {
        foreach(Task task in tasksToDelete)
            tasks.Remove(task);
        tasksToDelete.Clear();
        foreach (Task task in tasks.ToArray())
        {
            task.time += Time.fixedDeltaTime;
            task.t = task.time / task.goalTime;
            if (task.t < 1.0f)
            {
                UpdateTask(task);
            } else
            {
                task.t = 1f;
                UpdateTask(task);
                CompleteTask(task);
            }
        }
	}

    Task AddTask(float time, Easing.Ease easing)
    {
        return new Task()
        {
            t = 0f,
            time = 0f,
            goalTime = time,
            easing = easing,
            data = new Hashtable()
        };
    }

    public Task MoveTo(Transform obj, Vector3 posTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.move;
        task.data["transform"] = obj;
        task.data["from"] = obj.position;
        task.data["to"] = posTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task MoveToLocal(Transform obj, Vector3 posTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.moveLocal;
        task.data["transform"] = obj;
        task.data["from"] = obj.localPosition;
        task.data["to"] = posTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task MoveArcTo(Transform obj, Vector3 posTo, float arcHeight, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.moveArc;
        task.data["transform"] = obj;
        task.data["from"] = obj.position;
        task.data["to"] = posTo;
        task.data["arcHeight"] = arcHeight;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task MoveArcToLocal(Transform obj, Vector3 posTo, float arcHeight, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.moveArcLocal;
        task.data["transform"] = obj;
        task.data["from"] = obj.localPosition;
        task.data["to"] = posTo;
        task.data["arcHeight"] = arcHeight;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task ScaleTo(Transform obj, Vector3 scaleTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.scale;
        task.data["transform"] = obj;
        task.data["from"] = obj.localScale;
        task.data["to"] = scaleTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task RotateTo(Transform obj, Quaternion rotTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.rotate;
        task.data["transform"] = obj;
        task.data["from"] = obj.rotation;
        task.data["to"] = rotTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task RotateToLocal(Transform obj, Quaternion rotTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.rotateLocal;
        task.data["transform"] = obj;
        task.data["from"] = obj.localRotation;
        task.data["to"] = rotTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task ColorToMaterial(Material material, Color colTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.colorMaterial;
        task.data["material"] = material;
        task.data["from"] = material.color;
        task.data["to"] = colTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task ColorToSprite(SpriteRenderer sprite, Color colTo, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.colorSprite;
        task.data["sprite"] = sprite;
        task.data["from"] = sprite.color;
        task.data["to"] = colTo;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task ActionFloat(System.Action<float> action, float from, float to, float time, Easing.Ease easing, System.Action callback = null)
    {
        Task task = AddTask(time, easing);
        task.type = TaskType.actionFloat;
        task.data["action"] = action;
        task.data["from"] = from;
        task.data["to"] = to;
        task.callback = callback;
        tasks.Add(task);
        return task;
    }

    public Task DoAfter(float time, System.Action action) {
        Task task = AddTask(time, Easing.Ease.Linear);
        task.type = TaskType.timerSimple;
        task.callback = action;
        tasks.Add(task);
        return task;
    }

    public void CompleteTask(Task task)
    {
        if (task.callback != null)
            task.callback.Invoke();
        DeleteTask(task);
    }

    public void DeleteTask(Task task)
    {
        tasksToDelete.Add(task);
    }

    void UpdateTask(Task task)
    {
        switch (task.type)
        {
            case TaskType.move:
                ((Transform)task.data["transform"]).position = UpdateMove(
                    (Vector3)task.data["from"],
                    (Vector3)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.moveLocal:
                ((Transform)task.data["transform"]).localPosition = UpdateMove(
                    (Vector3)task.data["from"],
                    (Vector3)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.moveArc:
                ((Transform)task.data["transform"]).position = UpdateMoveArc(
                    (Vector3)task.data["from"],
                    (Vector3)task.data["to"],
                    (float)task.data["arcHeight"],
                    task.t,
                    task.easing);
                break;
            case TaskType.moveArcLocal:
                ((Transform)task.data["transform"]).localPosition = UpdateMoveArc(
                    (Vector3)task.data["from"],
                    (Vector3)task.data["to"],
                    (float)task.data["arcHeight"],
                    task.t,
                    task.easing);
                break;
            case TaskType.rotate:
                ((Transform)task.data["transform"]).rotation = UpdateRotate(
                    (Quaternion)task.data["from"],
                    (Quaternion)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.rotateLocal:
                ((Transform)task.data["transform"]).localRotation = UpdateRotate(
                    (Quaternion)task.data["from"],
                    (Quaternion)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.scale:
                ((Transform)task.data["transform"]).localScale = UpdateScale(
                    (Vector3)task.data["from"],
                    (Vector3)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.colorMaterial:
                ((Material)task.data["material"]).color = UpdateColor(
                    (Color)task.data["from"],
                    (Color)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.colorSprite:
                ((SpriteRenderer)task.data["sprite"]).color = UpdateColor(
                    (Color)task.data["from"],
                    (Color)task.data["to"],
                    task.t,
                    task.easing);
                break;
            case TaskType.actionFloat:
                System.Action<float> action = (System.Action<float>)task.data["action"];
                action(
                    Mathf.Lerp(
                        (float)task.data["from"],
                        (float)task.data["to"],
                        Easing.EaseResult(task.easing, 0f, 1f, task.t)
                        )
                    );
                break;
            default:
                break;
        }
    }

    Vector3 UpdateMove(Vector3 posFrom, Vector3 posTo, float t, Easing.Ease easing)
    {
        return Vector3.Lerp(posFrom, posTo, Easing.EaseResult(easing, 0f, 1f, t));
    }

    Vector3 UpdateMoveArc(Vector3 posFrom, Vector3 posTo, float arcHeight, float t, Easing.Ease easing)
    {
        Vector3 pos = Vector3.Lerp(posFrom, posTo, Easing.EaseResult(easing, 0f, 1f, t));
        pos.y += arcHeight * Mathf.Sin(Mathf.Clamp01(Easing.EaseResult(easing, 0f, 1f, t)) * Mathf.PI);
        return pos;
    }

    Vector3 UpdateScale(Vector3 scaleFrom, Vector3 scaleTo, float t, Easing.Ease easing)
    {
        return Vector3.Lerp(scaleFrom, scaleTo, Easing.EaseResult(easing, 0f, 1f, t));
    }

    Quaternion UpdateRotate(Quaternion rotFrom, Quaternion rotTo, float t, Easing.Ease easing)
    {
        return Quaternion.Lerp(rotFrom, rotTo, Easing.EaseResult(easing, 0f, 1f, t));
    }

    Color UpdateColor(Color colFrom, Color colTo, float t, Easing.Ease easing)
    {
        return Color.Lerp(colFrom, colTo, Easing.EaseResult(easing, 0f, 1f, t));
    }

    public enum TaskType {
        move,
        moveLocal,
        moveArc,
        moveArcLocal,
        rotate,
        rotateLocal,
        scale,
        colorMaterial,
        colorSprite,
        actionFloat,
        timerSimple
    };

    [System.Serializable]
    public class Task
    {
        public float t;
        public float goalTime;
        public float time;
        public Easing.Ease easing;

        public TaskType type;
        public Hashtable data;

        public Transform obj;
        public Material material;
        public SpriteRenderer sprite;

        public System.Action callback;
    }
}
