export default class FetchWrapper {
    constructor(baseURL) {
        this.URL = baseURL;
    }

    async get(endpoint) {
        return fetch(this.URL + endpoint, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        }).then(response => response.json());
    }

    async post(endpoint, body = {}) {
        return fetch(this.URL + endpoint, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(body)
        }).then(response => response.json());
    }
}