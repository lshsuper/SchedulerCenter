var utils = {

    ajax: function (vueTarget,url, params, fun) {
        axios({
            method: 'post',
            url: url,
            params: params,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(function (response) {
            fun && fun(response.data);
        }).catch(function (error) {
            if (error.response.status === 401) {
                return window.location.href = '/home/index';
            }
            vueTarget.$Message.success('出错啦!');
            console.log(error);
        });
    }

}