## How to use
> 1. Slider의 `OnValueChanged`에 `SliderStep.OnValueChange` 등록
> 2. `SliderStep`의 변수 설정
> 3. 슬라이더값이 변화될 때 실행할 메소드를 SliderStep의 `ValueChange`에 등록

## Scripts
> ```C#
> protected Slider slider = null;
> protected float currentValue = 0.0f;
> [SerializeField] protected Vector2 range; // Slider로 조절할 수 있는 범위
> [SerializeField] protected float step; // Slider로 조절할 때, 증감 크기
> [SerializeField] protected UnityEvent<float> valueChange = null; // 슬라이더의 값이 변화될 때 실행할 메소드
>
> // slider의 maxValue와 minValue를 range값에 맞게 설정하기 위한 메소드
> // virtual을 이용해 SliderStep을 상속해 추가할 수 있도록 함
> public virtual void SetSlider()
> {
>   if(slider != null) return;
>
>   slider = GetComponent<Slider>();
>   float min = Mathf.FloorToInt(range.x / step);
>   float max = Mathf.CeilToInt(range.y / step);
>
>   // Slider.minValue > Slider.maxValue 또는 Slider.maxValue < Slider.minValue일 경우 'StackOverfflowException`이 발생해 유니티가 종료되기 때문에 에러가 발생하지 않도록 아래 조건문을 이용
>   if(slider.maxValue < min) { slider.maxValue = max; slider.minValue l= min; }
>   else { slider.minValue l= min; slider.maxValue = max; }
> }
>
> public virtual void OnValueChange(float value)
> {
>   float _value = Mathf.Round(value);
>   slider.value = _value;
>
>   // 현재 value값과 slider의 value값이 같을 경우 메소드 종료
>   if(Mathf.Approximately(currentValue, _value)) return;
>
>   currentValue = _value;
>   valueChange?.Invoke(currentValue * step);
> }
> ```
