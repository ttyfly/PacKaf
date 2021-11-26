/*
 * Copyright (c) 2021 Lu Kangyuan (ttyfly@126.com)
 *
 * This file is part of PacKaf.
 *
 * PacKaf is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * PacKaf is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with PacKaf.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace PacKaf {
    public class PriorityQueue<T> {

        private List<(T value, int priority)> items;
        private bool descend;

        /// <summary>
        /// 优先队列。
        /// 若descend==true，则优先取出priority大的；反之取priority小的。
        /// </summary>
        /// <param name="descend">是否降序</param>
        public PriorityQueue(bool descend) {
            items = new List<(T, int)>();
            this.descend = descend;
        }

        public void Enqueue(T item, int priority) {
            int left = 0;
            int right = items.Count;

            int mid = (left + right) / 2;
            while (left < right) {
                if (priority == items[mid].priority) {
                    break;
                } else if (priority > items[mid].priority == descend) {
                    right = mid;
                } else {
                    left = mid + 1;
                }
                mid = (left + right) / 2;
            }
            items.Insert(mid, (item, priority));
        }

        public T Dequeue() {
            if (Empty) {
                throw new System.Exception("Cannot dequeue from an empty priority queue.");
            }
            T head = items[0].value;
            items.RemoveAt(0);
            return head;
        }

        public T Dequeue(out int priority) {
            if (Empty) {
                throw new System.Exception("Cannot dequeue from an empty priority queue.");
            }
            T head = items[0].value;
            priority = items[0].priority;
            items.RemoveAt(0);
            return head;
        }

        public bool Empty {
            get { return items.Count == 0; }
        }

        public void Clear() {
            items.Clear();
        }
    }
}