var FilterParams = {
    'category': "categoryId",
    'page': "pageIndex",
    'pageNumberPerPage': 'pageSize',
    "search": "search"
};

var domain = location.origin,
    url = location.href,
    url_clean = '';


function getOldFilterParams(params) {
    data = url.includes(params) ? url.split(`${params}=`)[1].split("&")[0] : null
    return data ? `${params}=${data}` : null
}

function getOldFilterStringParams() {
    var categoryClean = getOldFilterParams(FilterParams.category);
    var pageClean = getOldFilterParams(FilterParams.page);
    var pageNumberPerPageClean = getOldFilterParams(FilterParams.pageNumberPerPage);
    var searchClean = getOldFilterParams(FilterParams.search);


    $.each([pageClean, categoryClean, pageNumberPerPageClean, searchClean], function (item, value) {
        value ? url_clean += `&${value}` : null
    })

    url_clean = url_clean ? `?${url_clean.slice(1, url_clean.length)}` : url_clean
    return url_clean;
}



function generateNewUrl(event, params, value) {
    event.preventDefault();

    var result;
    oldNewString = getOldFilterStringParams();


    // url и парамент в начичии   --   заменяем значение
    // url в начичии, парамент отсутствует   --   добавляем ? и парамерт со значением
    // url и парамент в начичии   --   добавляем все с нуля
    if (oldNewString && oldNewString.includes(FilterParams[params])) {
        first = oldNewString.split(`${FilterParams[params]}`)[0]
        last = oldNewString.split(`${FilterParams[params]}`)[1].split("&")[1]
        result = last
            ? `${first}${FilterParams[params]}=${value}&${last}`
            : `${first}${FilterParams[params]}=${value}`
    } else if (oldNewString) {
        result = `${oldNewString}&${FilterParams[params]}=${value}`
    } else if (!oldNewString) {
        result = `?${FilterParams[params]}=${value}`
    }

    document.location.href = domain + '/' + result;    
}



