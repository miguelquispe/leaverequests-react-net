import type { components } from "@/api/generated/api-types";

type BaseResponse = components["schemas"]["ApiResponse"];

export type ApiResponse<T> = Omit<BaseResponse, "data"> & {
    data: T[] | null;
};