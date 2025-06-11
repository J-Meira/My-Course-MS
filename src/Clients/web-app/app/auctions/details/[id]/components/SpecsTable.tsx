"use client";

import { Table, TableBody, TableCell, TableRow } from "flowbite-react";

import { Auction } from "@/types";

type Props = {
  auction: Auction;
};

export const SpecsTable = ({ auction }: Props) => (
  <Table striped={true}>
    <TableBody className="divide-y">
      <TableRow className="bg-white">
        <TableCell className="whitespace-nowrap font-medium text-gray-900">
          Seller
        </TableCell>
        <TableCell>{auction.seller}</TableCell>
      </TableRow>
      <TableRow className="bg-white">
        <TableCell className="whitespace-nowrap font-medium text-gray-900">
          Make
        </TableCell>
        <TableCell>{auction.make}</TableCell>
      </TableRow>
      <TableRow className="bg-white">
        <TableCell className="whitespace-nowrap font-medium text-gray-900">
          Model
        </TableCell>
        <TableCell>{auction.model}</TableCell>
      </TableRow>
      <TableRow className="bg-white">
        <TableCell className="whitespace-nowrap font-medium text-gray-900">
          Year manufactured
        </TableCell>
        <TableCell>{auction.year}</TableCell>
      </TableRow>
      <TableRow className="bg-white">
        <TableCell className="whitespace-nowrap font-medium text-gray-900">
          Mileage
        </TableCell>
        <TableCell>{auction.mileage}</TableCell>
      </TableRow>
      <TableRow className="bg-white">
        <TableCell className="whitespace-nowrap font-medium text-gray-900">
          Has reserve price?
        </TableCell>
        <TableCell>{auction.reservePrice > 0 ? "Yes" : "No"}</TableCell>
      </TableRow>
    </TableBody>
  </Table>
);
