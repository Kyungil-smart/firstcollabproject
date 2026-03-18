## 기본 폴더 구조
```
Assets/
├─ _Imports/             
├─ _Settings/            URP/Input + 잡다한 유니티 세팅
├─ └─ Editor/            에디터 전용 스크립트 (빌드제외)
├─ Packages/
├─ Project/              핵심 폴더
│  ├─ Art/
│  │  ├─ Audio/
│  │  ├─ Materials/
│  │  ├─ Shader/
│  │  ├─ Sprites/        이미지: UI, 애니메이션 스프라이트 시트 포함
│  │  ├─ Texture/        이미지: 파티클/머티리얼용 + 잡다한거
│  │  └─ UI Toolkit/
│  ├─ DB/                스크립터블 오브젝트
│  ├─ Prefabs/
│  ├─ Scenes/
│  └─ Scripts/
├─ Resources/            Resources.Load()용 //파일이 많아지면 Addressable 고려
└─ WIP/                  테스트/개인용/정리전 오만가지 잡다한거 방치해도 무방한 폴더
```

 탐색과 관리가 좋은 폴더 구조를 목표로 함
개인 작업용 폴더 만들어도 무방함 (그라운드 룰을 지키기 좋음, WIP 폴더에 만들길 권장)
_Imports / Packages / Resources 폴더는 경로의존으로 변경 불가
외부 에셋등 .gitignore가 필요하면 _Imports, 개인 제작등 특별한 에셋은 Project/Art/ 폴더 사용


## _Imports 폴더의 패키지
- TextMeshPro

## 설치된 확장 패키지 (링크 첨부)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json):
- JSON 사용시 높은 확장성을 제공 (Json 배울때 사용했던 라이브러리)
- 각종 저장/로드에 유용
---
- [R3](https://github.com/Cysharp/R3):
- 반응형 프로그래밍을 위한 라이브러리 (옵져버패턴 배울때 ObervableProperty를 배웠는데 유사함)
- 값변경시 호출 로직, UI 바인딩에 유용
- [ObservableCollections](https://github.com/Cysharp/ObservableCollections):
- R3 전용 반응형 자료구조 라이브러리
---
- [UniTask](https://github.com/Cysharp/UniTask):
- 성능 좋은 async/await 기반 비동기 라이브러리
- 기본적인 사용법은 코루틴처럼 간단하지만 반환형이 가능하고 여러 고급 기능 보유
---
- [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween):
- 성능(개쩌는) struct 기반 트윈 애니메이션 라이브러리
- DOTween 대체용으로 만들어서 매우 유사하지만 약간의 차이 숙지 필요