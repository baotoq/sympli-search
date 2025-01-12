"use client";

import type { AuthProvider } from "@refinedev/core";

const API_URL = "http://localhost:5269";

export const authProvider: AuthProvider = {
  login: async ({ email, username, password, remember }) => {
    try {
      const response = await fetch(`${API_URL}/login`, {
        method: "POST",
        body: JSON.stringify({ email, password }),
        headers: {
          "Content-Type": "application/json",
        },
      });

      const { accessToken } = await response.json();

      localStorage.setItem("token", accessToken);

      return {
        success: true,
        redirectTo: "/",
      };
    } catch {
      return {
        success: false,
        error: {
          name: "LoginError",
          message: "Invalid username or password",
        },
      };
    }
  },
  logout: async () => {
    localStorage.removeItem("token");
    return {
      success: true,
      redirectTo: "/login",
    };
  },
  check: async () => {
    const auth = localStorage.getItem("token");
    if (auth) {
      return {
        authenticated: true,
      };
    }

    return {
      authenticated: false,
      logout: true,
      redirectTo: "/login",
    };
  },
  getPermissions: async () => {
    const auth = localStorage.getItem("token");
    if (auth) {
      return ["admin"];
    }
    return null;
  },
  getIdentity: async () => {
    const auth = localStorage.getItem("token");
    if (auth) {
      const response = await fetch(`${API_URL}/manage/info`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + auth,
        },
      });

      const { email } = await response.json();

      return {
        name: email,
        email: email,
        roles: ["admin"],
        avatar: "https://i.pravatar.cc/150?img=1",
      };
    }
    return null;
  },
  onError: async (error) => {
    if (error.status === 401 || error.status === 403) {
      return {
        logout: true,
        redirectTo: "/login",
        error,
      };
    }

    return { error };
  },
};
