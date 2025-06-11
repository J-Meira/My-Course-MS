"use client";

import { AiOutlineCar } from "react-icons/ai";

import { useParamsStore } from "@/hooks";
import { usePathname, useRouter } from "next/navigation";

export const Logo = () => {
  const router = useRouter();
  const pathname = usePathname();

  const { reset } = useParamsStore();

  const handleReset = () => {
    if (pathname !== "/") router.push("/");
    reset();
  };

  return (
    <div
      onClick={handleReset}
      className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500"
    >
      <AiOutlineCar size={34} />
      <div>Live Auctions</div>
    </div>
  );
};
