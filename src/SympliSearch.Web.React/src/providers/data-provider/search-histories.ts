"use client";

import { DataProvider } from "@refinedev/core";

const API_URL = "http://localhost:5413/api";

export const searchHistoriesDataProvider: DataProvider = {
  getOne: async ({ id, resource }) => {
    const response = await fetch(`${API_URL}/${resource}/${id}`);
    const data = await response.json();

    return {
      data,
    };
  },

  create: async ({ resource, variables }) => {
    throw new Error("Not implemented");
  },
  update: async ({ resource, id, variables }) => {
    throw new Error("Not implemented");
  },
  deleteOne: async () => {
    throw new Error("Not implemented");
  },
  getList: async ({ resource }) => {
    const auth = localStorage.getItem("token");
    const response = await fetch(`${API_URL}/${resource}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + auth,
      },
    });
    const data = await response.json();

    return {
      data: data.items,
      total: data.totalCount,
    };
  },
  getApiUrl: () => API_URL,
};
