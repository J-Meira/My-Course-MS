"use client";

import { usePathname, useRouter } from "next/navigation";
import { FaSearch } from "react-icons/fa";

import { useParamsStore } from "@/hooks";
import { ChangeEvent } from "react";

export const Search = () => {
  const router = useRouter();
  const pathname = usePathname();
  const { searchValue, setParams, setSearchValue } = useParamsStore();

  function onChange(event: ChangeEvent<HTMLInputElement>) {
    setSearchValue(event.target.value);
  }

  function search() {
    if (pathname !== "/") router.push("/");
    setParams({ searchTerm: searchValue });
  }

  return (
    <div className="flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm">
      <input
        onKeyDown={(e) => {
          if (e.key === "Enter") search();
        }}
        value={searchValue}
        onChange={onChange}
        type="text"
        placeholder="Search for cars by make, model or color"
        className="input-custom"
      />
      <button onClick={search}>
        <FaSearch
          size={34}
          className="bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2"
        />
      </button>
    </div>
  );
};
