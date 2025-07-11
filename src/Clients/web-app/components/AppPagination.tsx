"use client";

import { Pagination } from "flowbite-react";

type Props = {
  currentPage: number;
  pageCount: number;
  pageChanged: (page: number) => void;
};

export const AppPagination = ({
  currentPage,
  pageCount,
  pageChanged,
}: Props) => (
  <Pagination
    currentPage={currentPage}
    onPageChange={(e) => pageChanged(e)}
    totalPages={pageCount}
    layout="pagination"
    showIcons={true}
    className="text-blue-500 mb-5"
  />
);
