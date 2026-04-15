# 🍉 Unity6 Suika Game

Unity 6으로 제작한 수박게임(Suika Game) 스타일의 캐주얼 퍼즐 프로젝트입니다.
플레이어는 마우스로 과일을 움직여 떨어뜨리고, 같은 단계의 과일끼리 충돌하면 다음단계의 과일로 합성됩니다.
합성을 반복해 점수를 높이고, 떨어진 과일이 게임오버 라인에 닿으면 게임이 종료됩니다.
이 프로젝트에서는 과일 생성/드롭, 과일 합성, 점수 및 최고 점수 저장, 게임오버 UI, 로비 → 게임 씬의 자연스러운 전환까지 포함한 플레이 싸이클을 완성했습니다.

## 플레이 링크
https://usrrt.github.io/Unity6_SuikaGame/

---

## 📌 프로젝트 개요

- 프로젝트명 : Unity6 Suika Game
- 장르 : 캐주얼 퍼즐
- 개발 환경 : Unity 6 / C#, vs 2022
- 핵심 목표
    - 수박게임의 핵심 플레이 루프 구현
    - 드롭/합성/점수/게임오버 시스템 설계
    - UI와 씬 흐름까지 포함한 완성형 플레이 구조 구성

---

## 🎮 플레이 방식

1. Spawner가 마우스를 따라 좌우로 이동
2. 클릭시 현재 생성된 과일이 떨어짐
3. 같은 단계의 과일끼리 충돌하면 상위 단계 과일로 합성
4. 과일 합성 시 점수 누적
5. 떨어뜨린 과일 및 합성된 과일이 게임오버 라인에 닿으면 게임종료

---

## ✨ 주요 기능

### 1. 과일 생성 및 드롭 시스템

`FruitSpawnerHandler` 에서 현재 과일을 생성하고, 마우스 위치를 기반으로 Spawner를 좌우 이동시킵니다.
좌클릭 시 과일의 중력을 할당하여 낙하 하도록 만들었습니다.

---

### 2. 다음 과일 미리보기

다음에 만들 과일을 미리 정하고 UI에 표시하여 플레이어가 다음 수를 고려할 수 있도록 구성했습니다.
다음 과일 인덱스를 미리 뽑고, `GameManager` 의 `UpdatePreviewImage`메서드를 통해 미리보기 이미지를 갱신하는 식으로 구현하였습니다.

- 과일 배치 순서를 예측하며 플레이 가능

---

### 3. 동일 단계 과일 합성

`FruitMergeHandler`에서 충돌한 두 과일의 레벨을 비교하고, 같으면 중간 위치에 상위 단계 과일을 생성하도록 구현했습니다.
핵심 로직은 **“같은 레벨 충돌 → 중간 지점에 다음 레벨 생성 → 기존 두 과일 제거”** 입니다.

### 핵심 코드 예시

```csharp
    private void MergeFruit(FruitMergeHandler hitFruit)
    {
        var spawnPos = (hitFruit.transform.position + transform.position) / 2;

        var mergeFruit = Instantiate(
            _fruitSpawner.fruitPrefabs[++fruitLv],
            spawnPos,
            Quaternion.identity,
            GameManager.Instance.transform
        );

        mergeFruit.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        var fruitHandler = mergeFruit.GetComponent<FruitMergeHandler>();
        fruitHandler._fruitSpawner = _fruitSpawner;
        fruitHandler.IsDropped = true; // 새로 합성된 과일 처리

        Destroy(hitFruit.gameObject);
        Destroy(gameObject);
    }
```

- 두 과일의 평균 위치에 새 과일 생성
- 새 과일도 즉시 물리 영향을 받아 자연스럽게 낙하
- 합성과 동시에 점수 증가

---

### 4. 중복 합성 방지

충돌 이벤트는 양쪽 과일에서 동시에 들어올 수 있으므로, 한쪽만 합성 주체가 되도록 기준을 두었습니다. 
`GetEntityId` ****식별값 비교를 통해 중복 생성 문제를 방지했습니다.

### 핵심 코드 예시

```csharp
if (hitFruit.GetEntityId() < this.GetEntityId())
		return;
		
MergeFruit(hitFruit);
```

- 과일 합성시 합성 코드가 두 번 실행되는 문제 방지
- 상위 과일 중복 생성 방지

---

### 5. 점수 및 최고 점수 저장

`ScoreManager`는 현재 점수와 최고 점수를 관리하고, 최고 점수는 `PlayerPrefs`를 통해 저장합니다.

- 합성 결과에 따라 즉시 점수 반영
- 최고 점수는 재실행 후에도 유지

---

### 6. 게임오버 처리

`GameManager`는 박스에 들어간 과일만 게임오버 판정 대상으로 보고, 게임오버 발생 시 시간을 멈춘 뒤 UI 이벤트를 호출하도록 구성했습니다.

### 핵심 코드 예시

```csharp
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitFruit = collision.GetComponent<FruitMergeHandler>();
        if (hitFruit == null)
            return;
        if (!hitFruit.IsDropped)
		        return;
		        
        Time.timeScale=0f;
				gameOverAction?.Invoke();
        
    }
```

- Spawner에 대기중인 과일은 제외
- 실제 낙하가 완료, 합성된 과일만 게임오버 대상으로 처리
- 게임 정지와 UI 연출 시작 시점을 명확히 분리

---

### 7. 게임 오버 UI 연출

`UIHandler`는 게임오버 시 배경과 메뉴를 띄우고, **DOTween**으로 팝업을 연출합니다.
이때 `SetUpdate(true)`를 사용해 `Time.timeScale = 0` 상태에서도 UI 애니메이션은 계속 재생되도록 만들었습니다.

- 게임 로직은 멈추되 UI는 계속 자연스럽게 움직이게 처리

---

### 8. 씬 전환 시 자연스러운 연출

셰이더 그래프로 로비에서 게임씬으로 넘어갈때 화면 중앙에서 열리는 원형 마스크 전환 효과를 구현했습니다.

### 핵심 로직

- `Position`노드를 이용해 오브젝트 기준 좌표를 받아오고, `Subtract`와 `Lenght`노드를 사용해 중심으로부터의 거리를 계산하여 특정 픽셀이 기준값 밖인지 안인지 판별
- `Radius`프로퍼티를 기준값으로 사용해 투명원의 크기 제어
- `Step`노드를 이용한 경계 판정
    - 원 내부 0
    - 원 외부 1

---

## 🚧 트러블슈팅

### 합성 로직 중복 실행 문제

같은 단계 과일 충돌 시 양쪽 오브젝트에서 합성 코드가 동시에 실행될 수 있었습니다.

**해결**

- 충돌한 두 과일 중 한쪽만 합성 주체가 되도록 ID 비교 기준 적용

### 게임 정지 후 UI 애니메이션 정지 문제

게임오버 시 `Time.timeScale = 0`을 적용하면 결과 팝업 연출도 멈추는 문제가 있었습니다.

**해결**

- DOTween의 `SetUpdate(true)`를 사용해 UI는 별도 시간축으로 재생되도록 처리
