#!/bin/bash
# Script de Setup Automático - PetStay para Linux/macOS

set -e # Parar em caso de erro

echo "🚀 PetStay Setup Script"
echo "======================="
echo ""

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para imprimir com cores
print_status() {
	echo -e "${GREEN}✓${NC} $1"
}

print_error() {
	echo -e "${RED}✗${NC} $1"
}

print_warning() {
	echo -e "${YELLOW}⚠${NC} $1"
}

# Verificar .NET
echo "1️⃣  Verificando .NET SDK..."
if ! command -v dotnet &> /dev/null; then
	print_error ".NET SDK não encontrado"
	echo "Baixe em: https://dotnet.microsoft.com/download/dotnet/10.0"
	exit 1
fi

DOTNET_VERSION=$(dotnet --version)
print_status ".NET instalado: $DOTNET_VERSION"

# Verificar Git
echo ""
echo "2️⃣  Verificando Git..."
if ! command -v git &> /dev/null; then
	print_error "Git não encontrado"
	exit 1
fi
print_status "Git instalado"

# Clonar repositório (se ainda não existir)
if [ ! -d "PetStay" ]; then
	echo ""
	echo "3️⃣  Clonando repositório..."
	git clone https://github.com/laragoncalvs/PetStay.git
	print_status "Repositório clonado"
	cd PetStay
else
	print_warning "Diretório PetStay já existe"
	cd PetStay
fi

# Restaurar packages
echo ""
echo "4️⃣  Restaurando NuGet packages..."
dotnet restore
print_status "Packages restaurados"

# Verificar se é primeira execução
if [ ! -f "appsettings.local.json" ]; then
	echo ""
	echo "5️⃣  Criando configurações locais..."

	if [ -f "appsettings.json" ]; then
		cp appsettings.json appsettings.local.json
		print_status "appsettings.local.json criado"
	fi
fi

# Aplicar migrations
echo ""
echo "6️⃣  Aplicando migrations do banco de dados..."
if dotnet ef database update; then
	print_status "Migrations aplicadas com sucesso"
else
	print_warning "Erro ao aplicar migrations"
	echo "Por favor, configure ConnectionString em appsettings.json"
	echo "e tente novamente: dotnet ef database update"
fi

# Build
echo ""
echo "7️⃣  Compilando projeto..."
dotnet build
print_status "Projeto compilado"

echo ""
echo "🎉 Setup concluído com sucesso!"
echo ""
echo "Para iniciar a aplicação, execute:"
echo "  dotnet run"
echo ""
echo "A aplicação estará disponível em:"
echo "  http://localhost:5000 (HTTP)"
echo "  https://localhost:5001 (HTTPS)"
echo ""
echo "Credenciais padrão:"
echo "  Email: admin@petsstay.com"
echo "  Senha: Admin@123"
echo ""
print_warning "NÃO ESQUEÇA DE ALTERAR A SENHA DO ADMIN EM PRODUÇÃO!"
