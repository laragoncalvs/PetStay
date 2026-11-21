@echo off
REM Script de Setup Automático - PetStay para Windows

echo.
echo 🚀 PetStay Setup Script
echo =======================
echo.

REM Verificar .NET
echo 1️⃣  Verificando .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
	echo ✗ .NET SDK não encontrado
	echo Baixe em: https://dotnet.microsoft.com/download/dotnet/10.0
	pause
	exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo ✓ .NET instalado: %DOTNET_VERSION%

REM Verificar Git
echo.
echo 2️⃣  Verificando Git...
git --version >nul 2>&1
if errorlevel 1 (
	echo ✗ Git não encontrado
	echo Baixe em: https://git-scm.com/
	pause
	exit /b 1
)
echo ✓ Git instalado

REM Clonar repositório
if not exist "PetStay" (
	echo.
	echo 3️⃣  Clonando repositório...
	git clone https://github.com/laragoncalvs/PetStay.git
	if errorlevel 1 (
		echo ✗ Erro ao clonar repositório
		pause
		exit /b 1
	)
	echo ✓ Repositório clonado
)

cd PetStay

REM Restaurar packages
echo.
echo 4️⃣  Restaurando NuGet packages...
dotnet restore
if errorlevel 1 (
	echo ✗ Erro ao restaurar packages
	pause
	exit /b 1
)
echo ✓ Packages restaurados

REM Aplicar migrations
echo.
echo 5️⃣  Aplicando migrations do banco de dados...
dotnet ef database update
if errorlevel 1 (
	echo ⚠ Erro ao aplicar migrations
	echo Por favor, configure ConnectionString em appsettings.json
	echo e tente novamente: dotnet ef database update
	pause
) else (
	echo ✓ Migrations aplicadas com sucesso
)

REM Build
echo.
echo 6️⃣  Compilando projeto...
dotnet build
if errorlevel 1 (
	echo ✗ Erro ao compilar
	pause
	exit /b 1
)
echo ✓ Projeto compilado

REM Concluído
echo.
echo 🎉 Setup concluído com sucesso!
echo.
echo Para iniciar a aplicação, execute:
echo   dotnet run
echo.
echo A aplicação estará disponível em:
echo   http://localhost:5000 (HTTP)
echo   https://localhost:5001 (HTTPS)
echo.
echo Credenciais padrão:
echo   Email: admin@petsstay.com
echo   Senha: Admin@123
echo.
echo ⚠ NÃO ESQUEÇA DE ALTERAR A SENHA DO ADMIN EM PRODUÇÃO!
echo.
pause
