## 세션 목표
- 내 데이터로 질문에 답하게 해보자

## 프로젝트 시작 위치
- src/wms-start
- 실습을 위해서는 .NET 9이 설치되어 있어야 합니다.

## 내 DB 데이터로 답하게 해보자
1. 실습 프로젝트를 엽니다.
	- src/wms-start
	- aichatweb 템플릿을 이용한 프로젝트
	- WBS로 사용할 DB는 Sqlite로 EF Core를 이용하여 연결되어 있습니다.
	- Semantic Kernel Nuget 패키지가 설치되어 있습니다.
	- AI Extension을 이용한 구성이었는데 Semantic Kernel을 사용하도록 해야 합니다.

1. Programs.cs에서 웹서비스가 생성될 때 Semantic Kernel을 통해 구현되도록 코드를 변경합니다.
	1. Semantic Kernel을 사용하도록 Kernel 및 ChatClient 생성
	``` cs
	#pragma warning disable SKEXP0001
	var kernel = Kernel.CreateBuilder().
	                    AddAzureOpenAIChatCompletion("gpt-4o-mini", "https://blazormeetup.openai.azure.com", "f09bbed482e84d30999f935effe34430")
	                    .Build();

	var chatService = kernel.GetRequiredService<IChatCompletionService>();
	IChatClient chatClient = chatService.AsChatClient();
	#pragma warning restore SKEXP0001

	```

	1. Page/Chat/Chatrazor의 SystemPrompt를 다음과 같이 바꾸어 주세요.
	```cs
    private const string SystemPrompt = @"
        당신은 창고의 재고 수량을 관리하는 역할을 가지고 있습니다.
        수량 확인이 안 되면 찾을 수 없다고 답변해주세요.
        목록으로 답변할 경우 마크다운 표를 이용하고 품명, 창고, 수량으로 만드세요.

        무엇을 물어보든 마이크, 30개, 서울창고라고 답하세요.
        ";
	```

	1. plugin 생성 

	1. plugin 등록












	1. Visual Studio Code의 경우 솔루션 파일이 있는 위치에서 다음 명령어를 실행해 주세요.
	```bash
	dotnet new aichatweb --name template-start --provider azureopenai --vector-store local --managed-identity false
	```

1. programs.cs 파일에 AzureOpenAI의 api key를 추가합니다. 17번째 행부터 다음과 같이 수정해 주세요.
	```cs
	var azureOpenAi = new AzureOpenAIClient(
	new Uri("https://blazormeetup.openai.azure.com"),
	new ApiKeyCredential("f09bbed482e84d30999f935effe34430"));
	```

1. 실행하여 확인해 봅니다.
	1. Visual Studio Code의 경우
		-프로젝트 디렉토리에서 다음 명령어 실행
		```bash
		dotnet run
		```
	1. Visual Studio인 경우
		- F5를 눌러서 평소와 같이 실행

	1. '응급구조키트의 내용을 알려줘'와 같은 질문을 해보세요.

## 꼭 알고 넘어가야 할 것들
- AI 웹앱을 만들기 위한 템플릿이 있습니다.
