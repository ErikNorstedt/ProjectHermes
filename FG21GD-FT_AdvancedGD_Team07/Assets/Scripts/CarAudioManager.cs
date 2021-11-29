using UnityEngine;

public class CarAudioManager : MonoBehaviour
{
    private CarController car;
    public AudioSource IdleAudio;
    public AudioSource DrivingAudio;
    [Space]
    public AnimationCurve IdleAudioVolumeCurve;
    public AnimationCurve DrivingAudioVolumeCurve;
    public AnimationCurve DrivingAudioPitchCurve;

    float i;

    private void Awake() => car = GetComponent<CarController>();

    private void Update()
    {
        float speed = car.IsGrounded ? car.Speed : 1f;

        i = Mathf.MoveTowards(i, speed, Time.deltaTime * 60f);

        IdleAudio.volume = IdleAudioVolumeCurve.Evaluate(i) * 0.2f;
        DrivingAudio.volume = DrivingAudioVolumeCurve.Evaluate(i) * 0.2f;
        DrivingAudio.pitch = DrivingAudioPitchCurve.Evaluate(i);
    }
}
