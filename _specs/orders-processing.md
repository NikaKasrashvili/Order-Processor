# Spec for orders-processing

# Summary

Console app that loads `orders.json` and offers a simple interactive menu: show all orders, search orders by customer, show statistics over **completed** orders (count, total sales, average order value, most popular product by quantity). Business logic lives in a pure service operating on `IReadOnlyList<Order>`; file I/O is separate.

# Functional Requirements

- Load orders from `orders.json` located via `AppContext.BaseDirectory`; path overridable by first CLI argument.
- Interactive menu: `1` all orders, `2` search by customer, `3` statistics, `0` exit.
- FR1 Display all orders (id, customer, status, line items, order total), including cancelled ones.
- FR2 Find orders for a given customer.
- FR3 Statistics ignore every order whose status is not `completed`.
- FR4 Statistics over completed orders:
  - number of completed orders;
  - total sales = sum of `quantity * price` over all lines of completed orders;
  - average order value = total sales / completed order count;
  - most popular product = product with greatest summed `quantity` across completed orders.
- Order total = sum of `quantity * price` of its lines. Money uses `decimal`.

## Assumptions & Decisions

- Only status `completed` counts for statistics; any other status (`cancelled`, unknown) is excluded. Status is compared case-insensitively and trimmed.
- Customer search returns orders of **all** statuses; matching is case-insensitive, trimmed, exact (not substring).
- Order total is computed from lines; the file has no stored total.
- Most popular product is by summed **quantity**, not revenue and not number of orders.
- Tie for most popular product: return **all** tied products, sorted alphabetically.
- Average is not rounded in the service; it is rounded to 2 decimals for display only, using `MidpointRounding.AwayFromZero`.
- No completed orders: count 0, total 0, average 0 (no division by zero), most popular product none (printed "n/a").
- Empty orders list and orders with empty `items` are valid (order total 0).
- Missing `items` in the JSON is treated as an empty list (order total 0).
- Missing file or invalid JSON: print a clear error message and exit with a non-zero code; no stack trace.
- UI: simple interactive console menu (1 all orders, 2 search by customer, 3 statistics, 0 exit).

## Possible Edge Cases

- Tie for most popular product in the provided data (Keyboard 3 = Mouse 3).
- Cancelled order #2 contains Keyboard; wrongly including it gives 4 orders / 475 / 118.75 and Keyboard 4, which would hide the tie.
- Average is non-terminating (141.666…): integer division would give 141.
- Customer with several orders (Nino: 1 and 3); customer not found; empty input.
- Case/whitespace variants of status ("Completed", " CANCELLED ") and customer ("nino").
- Empty orders list; no completed orders; order with empty `items`.
- Missing file, invalid JSON.

## Out of Scope

Input data is trusted. Not handled: duplicate order ids, negative/zero quantity or price, product-name case variants, locale-specific number formatting.

## Acceptance Criteria

Expected results for the provided `orders.json` (computed by hand):

- Display all: 4 orders (ids 1–4), order totals 125, 50, 250, 50.
- Customer "Nino": orders 1 and 3. "Giorgi": order 2 (cancelled). "Ana": order 4. Unknown name: empty result.
- Completed orders: **3** (ids 1, 3, 4); cancelled order 2 excluded.
- Total sales: **425**.
- Average order value: **141.67** (141.666… unrounded).
- Quantity by product (completed only): Keyboard 3, Mouse 3, Monitor 1.
- Most popular product: **tie — Keyboard and Mouse (3 each)**.

## Open Questions

None — resolved as assumptions above.

## Testing Guidelines

Create tests in `tests/OrdersApp.Tests` for the following cases (at most 7, without going too heavy):

- Provided data statistics: count 3, total 425, average ≈ 141.67 (cancelled order excluded).
- Tie: top products are Keyboard and Mouse, sorted alphabetically.
- Unique top product (clear winner) when quantities differ.
- Customer search: multiple orders (Nino), case-insensitive/trimmed, includes cancelled (Giorgi), not found.
- Status matching is case-insensitive and trimmed ("Completed " counts).
- No completed orders (and empty list): zeros, no exception, no top product.
- Completed order with empty `items`: count 1, total 0, average 0, no top product, no exception.

## AI Review Notes
- Claude independently found every trap I had on my hand-made list:
  the Keyboard/Mouse tie, quantity vs revenue (Monitor would win by revenue),
  cancelled order #2 hiding the tie if wrongly included, and average rounding.
  Its hand-computed numbers (3 / 425 / 141.67) matched mine.
- First draft over-scoped edge cases (duplicate ids, negative quantities, locale)
  for a 60–90 min task — I moved these to "Out of Scope".
- First draft left decisions as open questions — I resolved them as explicit assumptions.
- Noted for implementation: C# `Math.Round` defaults to banker's rounding,
  so `MidpointRounding.AwayFromZero` must be explicit.
  - Implementation passed all 6 original tests but crashed on a spec'd edge case:
  a completed order with empty `items` made `Max()` run on an empty sequence
  (InvalidOperationException). Found in manual review, reproduced with a
  failing test first, then fixed with `DefaultIfEmpty(0)`.
- Initial tests missed two spec rules (unknown status excluded, whitespace
  customer search). Without the "pending" case, a `!= "cancelled"`
  implementation would have passed every test.
