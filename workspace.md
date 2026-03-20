# 그라운드 룰 (작성중)

1. 브랜치를 기능 및 인원 단위로 나누기
- 네이밍규칙: 접두사/기능_작성자 (예: feat/player-move_PEY)
- 통합후에는 새 프랜치를 만드는걸 기본으로 합니다
- 사용하지 않는 오래된 브랜치는 지워 혼동을 방지 합니다
---

2. 작업영역 준수
- 각자 작업중인 영역을 침범하지 않도록 합시다
- 공동사용 가능성이 높은 에셋을 변경하려면 팀원에게 알립니다
- 폴더 구조를 숙지하고 적절히 활용합시다
---

3. 커밋 메시지 규칙
- 대괄호 안에 접두사 [feat], [fix], [refactor] 등 영어로 작성 [참고링크](https://udacity.github.io/git-styleguide/)
- 작은 변경의 경우 제목으로 설명하고 본문작성은 생략해도 괜찮습니다
---

4. 풀 리퀘스트 규칙
- 메인에 직접 푸시를 지양하고 브랜치 단위의 병합을 기본으로 합니다
- 팀장이 보통 병합작업을 하지만 간단한 수정의 경우 개인의 판단으로 병합할 수 있지만 팀원에게 알려야 합니다
---

5. 코드 관리 규칙
- 변수/매개변수: camelCase, 클래스/메서드/구조체/열거형: PascalCase, 상수: UPPER_CASE
- private 의 경우 앞에 _ 언더바를 붙여서 표기한다
- 인터페이스: I로 시작, 추상클래스: Base로 끝남
- 네이밍 규칙을 준수하고 코드의 작명을 명확하게 하여 과한 주석을 피하고 가독성을 높입시다
- summary와 주석을 사용하여 핵심코드와 설명이 필요한 부분을 요약하고 설명합니다
---

6. 프리펩 관리 규칙
- 대분류는 폴더트리의 이름으로 잡고 세부는 '이름_속성_특이사항' 순서에서 최소 2가지 이상으로 정리합니다
- 예: [Prefabs/Characters/Enemy] 폴더의 Orc_Boss, [Prefabs/UI] 폴더의 Player_Hpbar_Blue)
- 많은 곳에 공통으로 적용되어야 하는 프리펩은 원본을 수정하고, 아니면 복사본/바리언트 로 정리합니다
---

7. 씬 관리 규칙
- 같은 씬을 두명의 작업자가 동시에 작업하지 않으며, 그래야 하면 복사하여 작업후 병합합니다
---

8. 데이터 정리
- Google Sheets 기반 웹 데이터를 파싱을 하는 것이 기본입니다
- _Imports 폴더의 에셋 공유는 Google Drive 시용합니다
- 세이브/로드 구현시 json, 간단한 설정은 PlayerPrefs 활용합니다
 ---

9. 작업 관리
- Notion에서 작업 상태를 구분하여 작성합니다
- 작업 분류: 예정 / 진행중 / 중단 / 완료
---

10. 커뮤니케이션 및 피드백
- 주 소통은 Discord에서 합니다
- QA 및 피드백은 GitHub Issues + Discord 피드백 채널을 만들어 활용합니다
---

# 기본 폴더 구조
```
Assets/
├─ Imports/             
├─ Packages/
├─ Project/              핵심 폴더
│  ├─ Art/
│  │  ├─ Audio/
│  │  ├─ Materials/
│  │  ├─ Shader/
│  │  ├─ Sprites/        이미지: UI, 스프라이트 시트 등
│  │  └─ Texture/        이미지: 파티클, 머티리얼 등
│  ├─ DB/                스크립터블 오브젝트
│  ├─ Prefabs/
│  ├─ Scenes/
│  └─ Scripts/
├─ Resources/            Resources.Load()용 //파일이 많아지면 Addressable 고려
├─ Settings/             URP/Input + 잡다한 유니티 세팅
├─ └─ Editor/            에디터 전용 스크립트 (빌드제외)
└─ WIP/                  테스트/개인용/정리전 오만가지 잡다한거 방치해도 무방한 폴더
```

 탐색과 관리가 좋은 폴더 구조를 목표로 함
- 개인 작업용 폴더 만들어도 무방함 (그라운드 룰 지키기 좋음, WIP 폴더에 만들길 권장)
- _Imports / Packages / Resources 폴더는 경로의존으로 변경 불가
- 외부 에셋등 .gitignore가 필요하면 _Imports, 개인 제작등 특별한 에셋은 Project/Art/ 폴더 사용


## Imports 폴더의 패키지
- TextMeshPro

## 설치된 확장 패키지 (링크 첨부)
[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- JSON 사용시 높은 확장성을 제공 (Json 배울때 사용했던 라이브러리)
- 각종 저장/로드에 유용
---
[R3](https://github.com/Cysharp/R3)
- 반응형 프로그래밍을 위한 라이브러리 (옵져버패턴 배울때 ObervableProperty를 배웠는데 유사함)
- 값변경시 호출 로직, UI 바인딩에 유용
- [ObservableCollections](https://github.com/Cysharp/ObservableCollections): R3 전용 반응형 자료구조 라이브러리
---
[UniTask](https://github.com/Cysharp/UniTask)
- 성능 좋은 async/await 기반 비동기 라이브러리
- 기본적인 사용법은 코루틴처럼 간단하지만 반환형이 가능하고 여러 고급 기능 보유
---
[PrimeTween](https://github.com/KyryloKuzyk/PrimeTween)
- 성능(개쩌는) struct 기반 트윈 애니메이션 라이브러리
- DOTween 대체용으로 만들어서 매우 유사하지만 약간의 차이 숙지 필요
---
