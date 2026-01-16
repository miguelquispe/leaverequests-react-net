import type { ApiResponse } from '@/core/types/api-response';
import axios, { type AxiosResponse } from 'axios';

const API_URL = import.meta.env.VITE_URL_API || 'http://localhost:7076/';

// Create axios instance
const apiClient = axios.create({
  baseURL: API_URL + 'api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Setup interceptors
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  const userId = localStorage.getItem('userId');
  const userRole = localStorage.getItem('userRole');
  
  if (token) config.headers.Authorization = `Bearer ${token}`;
  if (userId) config.headers['X-User-Id'] = userId;
  if (userRole) config.headers['X-User-Role'] = userRole;
  
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('userId');
      localStorage.removeItem('userRole');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Only for backend generated API calls
// Helper function to type responses
export const apiResponse = async <T>(request: Promise<AxiosResponse<unknown>>): Promise<ApiResponse<T>> => {
  const response = await request;
  return response.data as ApiResponse<T>;
}

export default apiClient;
