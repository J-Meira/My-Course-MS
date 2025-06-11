"use client";

import { Button } from "flowbite-react";
import { signIn } from "next-auth/react";

import { Heading } from "@/components";
import { useParamsStore } from "@/hooks";

type Props = {
  title?: string;
  subtitle?: string;
  showReset?: boolean;
  showLogin?: boolean;
  callbackUrl?: string;
};

export const DefaultsDisplay = ({
  callbackUrl,
  showLogin,
  showReset,
  subtitle = "Try changing or resetting the filter",
  title = "No matches for this filter",
}: Props) => {
  const reset = useParamsStore((state) => state.reset);

  return (
    <div className="h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg">
      <Heading title={title} subtitle={subtitle} center />
      <div className="mt-4">
        {showReset && (
          <Button outline onClick={reset}>
            Remove Filters
          </Button>
        )}
        {showLogin && (
          <Button
            outline
            onClick={() => signIn("id-server", { callbackUrl })}
            className="mt-2"
          >
            Login
          </Button>
        )}
      </div>
    </div>
  );
};
