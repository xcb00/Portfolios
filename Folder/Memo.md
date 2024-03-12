### Scroll View 상단으로 이동
`GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;`

### Slider를 정해진 크기만큼 증감
> - 유니티에서 기본 제공하는 Slider는 증감의 크기를 조절할 수 없기 때문에 증감의 크기를 조절할 수 있는 슬라이더를 구현함
>   > <details>
>   > <summary>Show Code</summary>
>   > 
>   > ```C#
>   > float currentValue = 0.0f;
>   > Slider slider = null;
>   > public Vector2 range; // Slider의 범위
>   > public float step = 0.1f; // Slider의 증감 크기
>   > public UnityEvent<float> valueChange = null; // Slider의 값이 변화될 때 실행할 메소드
>   > 
>   > void Start()
>   > {
>   >   if(slider == null) slider = GetComponent<Slider>();
>   >   // slider의 범위를 재설정
>   >   slider.minValue = Mathf.FloorToInt(range.x / step);
>   >   slider.maxValue = Mathf.CeilToInt(range.y / step);
>   >   currentValue = slider.value;
>   > }
>   > 
>   > public void OnValueChange(float value)
>   > {
>   >   // Slider의 value값을 반올림
>   >   float _v = Mathf.Round(value);
>   >   slider.value = _v;
>   > 
>   >   // 반올림한 vaule값과 currentValue값이 같을 경우 메소드 종료
>   >   if(Mathf.Approximately(currentValue, _v)) return; 
>   > 
>   >   currentValue = _v;
>   >   valueChange?.Invoke(Mathf.Clamp(currentValue * step, range.x, range.y));
>   > }
>   > ```
>   > </details>

