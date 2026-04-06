import { useCallback, useState } from "react";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";

export function useAsyncTask<TArgs extends unknown[], TData>(task: (...args: TArgs) => Promise<TData>) {
  const [state, setState] = useState<AsyncState<TData>>(createAsyncState<TData>("idle"));

  const execute = useCallback(
    async (...args: TArgs): Promise<TData> => {
      setState(createAsyncState<TData>("loading"));

      try {
        const data = await task(...args);
        setState(createAsyncState<TData>("success", data));
        return data;
      } catch (error: unknown) {
        const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred.";
        setState(createAsyncState<TData>("error", null, errorMessage));
        throw error;
      }
    },
    [task]
  );

  return {
    ...state,
    execute
  };
}
