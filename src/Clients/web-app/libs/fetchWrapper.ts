import { auth } from "@/auth";

const baseUrl = "http://localhost:6001/";

const get = async (url: string) => {
  const requestOptions = {
    method: "GET",
    header: await getHeaders(),
  };

  const response = await fetch(baseUrl + url, requestOptions);
  return await handleResponse(response);
};

const post = async (url: string, body: {}) => {
  const requestOptions = {
    method: "POST",
    headers: await getHeaders(),
    body: JSON.stringify(body),
  };
  const response = await fetch(baseUrl + url, requestOptions);
  return await handleResponse(response);
};

const put = async (url: string, body: {}) => {
  const headers = await getHeaders();
  console.log(headers);
  const requestOptions = {
    method: "PUT",
    headers: await getHeaders(),
    body: JSON.stringify(body),
  };
  const response = await fetch(baseUrl + url, requestOptions);
  return await handleResponse(response);
};

const del = async (url: string) => {
  const requestOptions = {
    method: "DELETE",
    headers: await getHeaders(),
  };
  const response = await fetch(baseUrl + url, requestOptions);
  return await handleResponse(response);
};

const getHeaders = async () => {
  const session = await auth();
  const headers = { "Content-type": "application/json" } as any;
  if (session) {
    headers.Authorization = `Bearer ${session.accessToken}`;
  }
  return headers;
};

const handleResponse = async (response: Response) => {
  const text = await response.text();
  const data = text && JSON.parse(text);

  if (response.ok) {
    return data || response.statusText;
  } else {
    const error = {
      status: response.status,
      message: response.statusText,
    };
    console.error(error);
    return { error };
  }
};

export const fetchWrapper = {
  get,
  post,
  put,
  del,
};
