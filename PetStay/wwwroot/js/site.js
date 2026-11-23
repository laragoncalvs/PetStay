
document.addEventListener('DOMContentLoaded', () => {
    const formatarTelefone = (valor) => {
        const numeros = valor.replace(/\D/g, '').slice(0, 11);
        if (!numeros) return '';
        if (numeros.length <= 2) return `(${numeros}`;
        const ddd = numeros.slice(0, 2);
        const telefone = numeros.slice(2);
        if (telefone.length <= 4) return `(${ddd}) ${telefone}`;
        const posicaoHifen = telefone.length > 8 ? 5 : 4;
        return `(${ddd}) ${telefone.slice(0, posicaoHifen)}-${telefone.slice(posicaoHifen)}`;
    };
    document.querySelectorAll('[data-mascara-telefone]').forEach((campo) => {
        campo.value = formatarTelefone(campo.value);
        campo.addEventListener('input', () => {
            campo.value = formatarTelefone(campo.value);
        });
    });
});


document.addEventListener('DOMContentLoaded', () => {
    const entrada = document.getElementById('dataEntrada');
    const saida = document.getElementById('dataSaida');
    if (!entrada || !saida) return;

    function corrigirSaida() {
        if (saida.value && saida.min && saida.value < saida.min) {
            saida.value = saida.min;
            saida.dispatchEvent(new Event('change', { bubbles: true }));
        }
    }

    function atualizarDataMinima() {
        if (!entrada.value) {
            saida.removeAttribute('min');
            return;
        }
        const data = new Date(`${entrada.value}T00:00:00Z`);
        data.setUTCDate(data.getUTCDate() + 1);
        saida.min = data.toISOString().slice(0, 10);
        corrigirSaida();
    }

    entrada.addEventListener('change', atualizarDataMinima);
    saida.addEventListener('change', corrigirSaida);
    atualizarDataMinima();
});
