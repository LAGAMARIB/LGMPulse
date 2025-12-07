function LGM_ConvertToDecimal(strValor) {
    var valor = parseFloat(strValor.replace(/\./g, '').replace(/\,/g, '.'));
    return valor;
}

function LGM_DecimalToString(valor, decs=2) {
    return valor.toFixed(decs).replace(/\./g, ',').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
}

function LGM_DecimalToMVC(valor, decs=2) {
    return valor.toFixed(decs).replace(/\./g, ',');
}

function LGM_DateTimeToString(jsonDateTime) {
    var result = '';
    if (jsonDateTime) {
        var date = new Date(parseInt(jsonDateTime.substr(6)));
        result = moment(date).format('DD/MM/YYYY hh:mm:ss');
    }
    return result;
}

function LGM_DateTimeToShortString(jsonDateTime) {
    var result = '';
    if (jsonDateTime) {
        var date = new Date(parseInt(jsonDateTime.substr(6)));
        result = moment(date).format('DD/MM/YYYY');
    }
    return result;
}
